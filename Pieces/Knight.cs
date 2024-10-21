using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace ChessNEA
{
    class Knight : Piece
    {
        public Knight(Board board, bool iswhite, Rectangle position)
        {
            IsWhite = iswhite;
            Position = position;
        }

        public override void LoadContent(ContentManager content)
        {
            if (IsWhite)
            {
                pieceSprite = content.Load<Texture2D>("KnightW");
            }
            else
            {
                pieceSprite = content.Load<Texture2D>("KnightB");
            }
            //This loads the sprite depending on if the piece is white or not
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
                Debug.WriteLine(IsWhite ? "Mouse has clicked white knight" : "Mouse has clicked black knight");

                findMoves();

            }
            void findMoves()
            {
                legalmoves.Clear();
                int row = (Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
                int col = (Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle

                
                    for (int j = -1; j <= 1; j = j + 2)
                    {
                        for (int i = 2; i >= -2; i = i - 4)
                        {
                            if (row + i >= 0 && row + i < 8 && col + j >= 0 && col + j < 8)
                            {
                                if (board.ChessBoard[row + i, col + j] == null || board.ChessBoard[row + i, col + j].IsWhite != this.IsWhite) //checks if the square is empty or has an enemy piece
                                {
                                        legalmoves.Add(new Point(col + j, row + i));
                                }
                               
                            }

                            if (col + i >= 0 && col + i < 8 && row + j >= 0 && row + j < 8)
                            {
                                if (board.ChessBoard[row + j, col + i] == null || board.ChessBoard[row + j, col + i].IsWhite != this.IsWhite)//checks if the square is empty or has an enemy piece
                                {
                                    legalmoves.Add(new Point(col + i, row + j));
                                }
                                
                            }
                        }
                    }
                movescalculated = true;



            }
        }

    }
}
