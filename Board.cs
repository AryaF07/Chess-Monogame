using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessNEA
{
    public class Board
    {
        public Piece[,] ChessBoard;
        Texture2D highlightSprite;
        List<Rectangle> highlights = new List<Rectangle>(); //this list will contain the rectangles for the move highlights
        bool highlightsDrawn;
        bool leftclickPressed;
        //This array will track the location of all the pieces throughout the game as would a normal board would.
        public Board()
        {
            ChessBoard = new Piece[8, 8];
            SetupBoard();
            Piece.setBoard(this);
            
            //Function is called if board is instantiated,
        } 

        public void SetupBoard()
        {
            ChessBoard[0, 0] = new Rook(this,false, new Rectangle(165, 5, 50, 50));
            ChessBoard[0, 1] = new Knight(this, false, new Rectangle(225, 5, 50, 50));
            ChessBoard[0, 2] = new Bishop(this, false, new Rectangle(285, 5, 50, 50));
            ChessBoard[0, 3] = new Queen(this, false, new Rectangle(345, 5, 50, 50));
            ChessBoard[0, 4] = new King(this, false, new Rectangle(405, 5, 50, 50));
            ChessBoard[0, 5] = new Bishop(this, false, new Rectangle(465, 5, 50, 50));
            ChessBoard[0, 6] = new Knight(this, false, new Rectangle(525, 5, 50, 50));
            ChessBoard[0, 7] = new Rook(this, false, new Rectangle(585, 5, 50, 50));
            //black pieces

            ChessBoard[7, 0] = new Rook(this, true, new Rectangle(165, 425, 50, 50));
            ChessBoard[7, 1] = new Knight(this, true, new Rectangle(225, 425, 50, 50));
            ChessBoard[7, 2] = new Bishop(this, true, new Rectangle(285, 425, 50, 50));
            ChessBoard[7, 3] = new Queen(this, true, new Rectangle(345, 425, 50, 50));
            ChessBoard[7, 4] = new King(this, true, new Rectangle(405, 425, 50, 50));
            ChessBoard[7, 5] = new Bishop(this, true, new Rectangle(465, 425, 50, 50));
            ChessBoard[7, 6] = new Knight(this, true, new Rectangle(525, 425, 50, 50));
            ChessBoard[7, 7] = new Rook(this, true, new Rectangle(585, 425, 50, 50));
            //white pieces

            // This puts each chess piece in their positions same as a regular chess board
            
           
            for (int i = 0; i < 8; i++)
            {
                
                ChessBoard[1, i] = new Pawn(this,false, new Rectangle(165 + (i*60), 65, 50, 50)); //white pawns

                ChessBoard[6, i] = new Pawn(this, true, new Rectangle(165 + (i*60), 365, 50, 50)); // black pawns
               
                //each square is separated by 60 so multiplier increases by 60 for the next pawns
            }
            // this puts the pawns in their positions, for loop is used as pawns are on the same rows
        }
        public void LoadContent(ContentManager content)
        {
            //loads all the sprites that are in the ChessBoard array
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null)
                {
               
                    piece.LoadContent(content);
                    //If the space in the array isn't empty, it will call that piece's draw method
                }
                
            }
            highlightSprite = content.Load<Texture2D>("highlight");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draws all the sprites that are in the ChessBoard array
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null)
                {
                    
                    piece.Draw(spriteBatch);
                    //If the space in the array isn't empty, it will call that piece's draw method
                }

                
            }
            if (highlights.Count > 0) // If the list is not empty
            {
                foreach (Rectangle rect in highlights)
                {
                    spriteBatch.Draw(highlightSprite, rect,Color.White * 0.5f); //draws every highlight in the list
                }
                highlightsDrawn = true;
                
            }
        }

        public void highlightSquares(List<Point> points)
        {
           
            for (int i = 0; i < points.Count; i++)
            {
                //X is column
                //Y is row
                int x = points[i].X; //Stores the X coordinate of the point to an integer variable
                int y = points[i].Y;   //Stores the Y coordinate of the point to an integer variable
                highlights.Add( new Rectangle(160 + 60 * x,  60*y, 60, 60)); //finds the coordinates for the rectangle using the x and y of the points
                Debug.WriteLine("Highlighted");
                
               
            }
          
        }
        int moveRow = 0; //Will be used to store the row number for the piece
        int moveColumn = 0;
        //Will be used to store the row and column numbers for the piece
        public void Update()
        {
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null) 
                {
                    piece.Update(); //updates the pieces to check if they have been clicked

                    if (piece.movescalculated == true)
                    {
                        highlights.Clear();
                        highlightSquares(piece.legalmoves); //if they have been clicked and legal moves were found, these moves are highlighted.
                        moveRow = (piece.Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
                        moveColumn = (piece.Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle
                        piece.movescalculated = false; //this is to prevent the moves for this piece being highlighted more than once
                        piece.legalmoves.Clear(); //Clears the legal move list as they have all been highlighted

                    }



                    if (highlightsDrawn == true)  //checks if the highlights have been drawn before checking if they have been clicked
                    {
                        foreach (Rectangle rect in highlights)
                        {
                            MouseState mouse = Mouse.GetState();

                            //gets the current state of the mouse for example XY position and button states
                            Rectangle mouse2 = new Rectangle(mouse.X, mouse.Y, 1, 1);
                            //creates rectangle with the position of the mouse
                            if (mouse2.Intersects(rect)) //Checks if the mouse is over the highlight
                            {
                                int col = ((rect.X - 160) / 60); //Finds the column number of the highlight
                                int row = (rect.Y / 60); //Finds  the row number of the highlight
                               

                                if (leftclickPressed == false && mouse.LeftButton == ButtonState.Pressed)
                                {
                                    leftclickPressed = true;
                                 
                                }
                                if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released) //If clicked
                                {
                                  
                                    leftclickPressed = false; //Sets to false for the next frame

                                    //Rearranged the calculation in line 119
                                    ChessBoard[moveRow, moveColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                    ChessBoard[row, col] = ChessBoard[moveRow, moveColumn]; //Changes the position of the piece in the array after the move has been made
                                    ChessBoard[moveRow, moveColumn] = null; //previous position is empty

                                    highlights.Clear(); //Clears the highlights because a move has been made
                                    highlightsDrawn = false; //Move has been made
                                    break; //Stops the search


                                }
                            }
                        }

                    } 




                }
                
            }

            
        }

    }
}
