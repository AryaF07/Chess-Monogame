using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessNEA
{
     class Bishop : Piece
    {
        public Bishop(Board board, bool iswhite, Rectangle position) 
        {

            IsWhite = iswhite;
            Position = position;
        }

        public override void LoadContent(ContentManager content)
        {
            if (IsWhite)
            {
                pieceSprite = content.Load<Texture2D>("BishopW");
                //if the bishop is white load the white sprite

            }
            else
            {
                pieceSprite = content.Load<Texture2D>("BishopB");
                //if the bishop is black load the black sprite
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
                Debug.WriteLine(IsWhite ? "Mouse has clicked white bishop" : "Mouse has clicked black bishop");
                //Displays line depending on if the piece is white or not.
                findMoves();
            }
        }

        public override void findMoves()
        {
            legalmoves.Clear();
            int row = (Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
            int col = (Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle
         
            for (int i = 1; i <= 7; i++) //Checks top right diagonal
            {
                if (row - i >= 0 && col + i < 8)
                {
                    if (board.ChessBoard[row - i, col + i] == null) //Checks if square has no piece on it
                    {
                        legalmoves.Add(new Point(col + i, row - i));
                    }
                    else if (board.ChessBoard[row - i, col + i].IsWhite != this.IsWhite) //Checks if square has enemy piece
                    {
                        legalmoves.Add(new Point(col + i, row - i));
                        break; //Stops search to prevent a move past a piece
                    }
                    else
                    {
                        break; //breaks if there is a piece with the same colour as bishop
                    }
                }
            }

            for (int i = 1; i <= 7; i++) //Checks bottom right diagonal
            {
                if (row + i < 8 && col + i < 8)
                {
                    if (board.ChessBoard[row + i, col + i] == null)
                    {
                        legalmoves.Add(new Point(col + i, row + i));
                    }
                    else if (board.ChessBoard[row + i, col + i].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col + i, row + i));
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int i = 1; i <= 7; i++) //Checks top left diagonal
            {
                if (row - i >= 0 && col - i >= 0)
                {
                    if (board.ChessBoard[row - i, col - i] == null)
                    {
                        legalmoves.Add(new Point(col - i, row - i));
                    }
                    else if (board.ChessBoard[row - i, col - i].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col - i, row - i));
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int i = 1; i <= 7; i++) //Checks bottom left diagonal
            {
                if (row + i < 8 && col - i >= 0)
                {
                    if (board.ChessBoard[row + i, col - i] == null) 
                    {
                        legalmoves.Add(new Point(col - i, row + i));
                    }
                    else if (board.ChessBoard[row + i, col - i].IsWhite != this.IsWhite) 
                    {
                        legalmoves.Add(new Point(col - i, row + i));
                        break; 
                    }
                    else
                    {
                        break; 
                    }
                }
            }
            movescalculated = true;
        }
    }
}
