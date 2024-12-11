using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using SharpDX.DirectWrite;

namespace ChessNEA
{
    class Pawn : Piece
    {
        public Pawn(Board board, bool iswhite, Rectangle position)
        { //initialises the properties as the object is being created
            //allows the pawn class to refer to the board class to access the array
            IsWhite = iswhite;
            Position = position;
        
        }


       public bool movedtwoSquares = false;
        public override void LoadContent(ContentManager content)
        {
            if (IsWhite)
            {
                pieceSprite = content.Load<Texture2D>("PawnW");
                //Loads the white sprite if "IsWhite" is true
            }
            else
            {
                pieceSprite = content.Load<Texture2D>("PawnB");
                //Loads the black sprite if "IsWhite" is false
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

            if (mouse2.Intersects(Position)) 
                {
                if (leftclickPressed == false && mouse.LeftButton == ButtonState.Pressed)
                {
                    leftclickPressed = true;
                    //If you have not pressed left click in the previous frame and your mouse is over the sprite and have pressed it in the current frame pressed is true
                }

                if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released)
                {
                    // if you have pressed left click in the previous frame and have now released left click, there will be an output
                    leftclickPressed = false;
                    Debug.WriteLine(IsWhite ? "Mouse has clicked white pawn" : "Mouse has clicked black pawn");
                    
                    findMoves(); //Calls function to find the legal moves of the piece that has been clicked

                }
            }
          
        }
        
        void findMoves()
        {
            
            legalmoves.Clear();
            int row = (Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
            int col = (Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle
            if (IsWhite) // logic for white pieces
            {
                
                if (row == 6) //If the pawn is at the start 
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        if (board.ChessBoard[row-i, col] == null)
                        {
                            legalmoves.Add(new Point(col, row - i)); //adds move
                        }
                        else
                        {
                            break; //if there is a piece infront of the pawn stop the search
                        }
                    }
                    //adds moves to the legalmoves list of the piece
                    
                   
                }
                if (row-1>=0) //Checks if index isnt outside the array
                {
                    if (row !=6 && board.ChessBoard[row - 1, col] == null) //Pawn can only move 1 square at a time if its not at the start
                    {
                        legalmoves.Add(new Point(col, row - 1));
                        
                    }

                    if (col - 1 >= 0 && board.ChessBoard[row - 1, col - 1] != null && board.ChessBoard[row - 1, col - 1].IsWhite != this.IsWhite) //Checks if index is outside the array and checks if the piece on that square is from the opposing colour
                    {
                        legalmoves.Add(new Point(col - 1, row - 1));
                        
                    }
                    if (col + 1 < 8 && board.ChessBoard[row - 1, col + 1] != null && board.ChessBoard[row - 1, col + 1].IsWhite != this.IsWhite)//Checks if index is outside the array and checks if the piece on that square is from the opposing colour
                    {
                        legalmoves.Add(new Point(col +1, row -1));
                        
                    }
                }
                //EN PASSANT
                if (col - 1 >= 0)
                {
                    if (board.ChessBoard[row, col - 1] is Pawn pawn && pawn.movedtwoSquares == true && board.ChessBoard[row, col - 1].IsWhite != this.IsWhite) //checks if the square to the left is an enemy pawn
                    {
                        legalmoves.Add(new Point(col - 1, row - 1));
                    }
                }
                if (col + 1 <8)
                {
                    if (board.ChessBoard[row, col + 1] is Pawn pawn && pawn.movedtwoSquares == true && board.ChessBoard[row, col + 1].IsWhite != this.IsWhite) //checks if the square to the right is an enemy pawn
                    {
                        legalmoves.Add(new Point(col + 1, row - 1));
                    }
                }


            }
            else //logic for black pieces
            {
                if (row == 1)  //If the pawn is at the start 
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        if (board.ChessBoard[row + i, col] == null)
                        {
                            legalmoves.Add(new Point(col, row + i));
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                }
                if (row + 1 < 8) //Checks if index isnt outside the array
                {
                    if (row != 1 && board.ChessBoard[row + 1, col] == null) //Pawn can only move 1 square at a time if its not at the start
                    {
                        legalmoves.Add(new Point(col, row + 1));
                       
                    }

                    if (col - 1 >= 0 && board.ChessBoard[row + 1, col - 1] != null && board.ChessBoard[row + 1, col - 1].IsWhite != this.IsWhite)//Checks if index is outside the array and checks if the piece on that square is from the opposing colour
                    {
                        legalmoves.Add(new Point(col - 1, row + 1));
                        
                    }
                    if (col + 1 < 8 && board.ChessBoard[row + 1, col + 1] != null && board.ChessBoard[row + 1, col + 1].IsWhite != this.IsWhite)//Checks if index is outside the array and checks if the piece on that square is from the opposing colour
                    {
                        legalmoves.Add(new Point(col + 1, row + 1));
                       
                    }

                }
                //EN PASSANT
                if (col - 1 >= 0)
                {
                    if (board.ChessBoard[row, col - 1] is Pawn pawn && pawn.movedtwoSquares == true && board.ChessBoard[row, col - 1].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col - 1, row + 1));
                    }
                }
                if (col + 1 < 8)
                {
                    if (board.ChessBoard[row, col + 1] is Pawn pawn && pawn.movedtwoSquares == true && board.ChessBoard[row, col + 1].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col + 1, row + 1));
                    }
                }
            }

            movescalculated = true;
        }
    }
}
