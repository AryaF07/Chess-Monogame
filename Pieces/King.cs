using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessNEA
{
   public class King : Piece
    {
        public King(Board board, bool iswhite, Rectangle position) 
        {
            IsWhite = iswhite;
            Position = position;
        }
       public bool hasMoved = false;
        bool testing = true;
        public override void LoadContent(ContentManager content)
        {
            if (IsWhite)
            {
                pieceSprite = content.Load<Texture2D>("KingW");
            }
            else
            {
                pieceSprite = content.Load<Texture2D>("KingB");
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pieceSprite, Position, Color.White);
            //draws the chess piece sprite on the screen
        }
        public override void Update()
        {
          
            MouseState mouse = Mouse.GetState();
            //gets the current state of the mouse for example XY position and button states
            Rectangle mouse2 = new Rectangle(mouse.X, mouse.Y, 1, 1);
            //creates rectangle with the position of the mouse

            if (leftclickPressed == false && mouse2.Intersects(Position) && mouse.LeftButton == ButtonState.Pressed)
            {
                leftclickPressed = true;
                //If you have not pressed left click in the previous frame and your mouse is over the sprite and have pressed it in the current frame pressed is true
            }

            if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released)
            {
                // if you have pressed left click in the previous frame and have now released left click, there will be an output
                leftclickPressed = false;
                
                //Displays line depending on if the piece is white or not.
                findMoves();
            }
       
            
        }

         public override void findMoves()
        {
            legalmoves.Clear();
            int row = (Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
            int col = (Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle

            for (int i = -1; i <= 1; i++)
            {
                if (row + i >= 0 && row + i < 8 && col - 1 >= 0)
                {
                    if (board.ChessBoard[row + i, col - 1] == null || board.ChessBoard[row + i, col - 1].IsWhite != this.IsWhite) //checks if the square is empty or if it has an enemy piece
                    {
                        legalmoves.Add(new Point(col - 1, row + i));
                    }
                   
                }
                if (row + i >= 0 && row + i < 8)
                {
                    if (board.ChessBoard[row + i, col] == null || board.ChessBoard[row + i, col].IsWhite != this.IsWhite)  //checks the squares in the same column as the king
                    {
                        legalmoves.Add(new Point(col, row + i));
                    }
                }
                if (row + i >= 0 && row + i < 8 && col + 1 <8)
                {
                    if (board.ChessBoard[row + i, col + 1] == null || board.ChessBoard[row + i, col + 1].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col + 1, row + i));
                    }
                }
            }

            if (hasMoved == false && board.check == false) //Checks if the king has not moved and if the board is not in check
            {
                if (board.ChessBoard[row, col - 1] == null && board.ChessBoard[row, col - 2] == null && board.ChessBoard[row, col - 3] == null) 
                { //Checks if the 3 squares between the rook and king to the left are empty for a queenside castle
                    if (board.ChessBoard[row,0] is Rook rook && rook.hasMoved == false)//Checks if the left rook has not moved
                    {
                        for (int j = 1; j <= 2; j++)
                        {
                            Position = new Rectangle(165 + (60 * col - j), 5 + (60 * row), 50, 50);
                            if (board.IsKingInCheck(this) == true) //Checks if the king would be in check in the squares that it moves through
                            {
                                break; //No legal moves added if the king is in check in one of the squares
                            }
                            if (j == 2)
                            {
                                legalmoves.Add(new Point(col - 2, row)); 
                            }
                        }
                    }
                }
                else if (board.ChessBoard[row, col + 1] == null && board.ChessBoard[row, col + 2] == null)
                {//Checks if the 2 squares between the rook and king to the right are empty for a kingside castle
                    if (board.ChessBoard[row,7] is Rook rook && rook.hasMoved == false)
                    {
                        for (int j = 1; j <= 2; j++)
                        {
                            Position = new Rectangle(165 + (60 * col + j), 5 + (60 * row), 50, 50);
                            if (board.IsKingInCheck(this) == true)
                            {
                                break;
                            }
                            if (j == 2)
                            {
                                legalmoves.Add(new Point(col + 2, row));
                            }
                        }
                    }
                }


            }
            Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50);
            movescalculated = true;
        }

     
    }
}
