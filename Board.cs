using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
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
      public bool turn = true; //true = whites turn false = blacks turn
        public bool check = false;
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
                    spriteBatch.Draw(highlightSprite, rect, Color.White * 0.5f); //draws every highlight in the list
                }
                highlightsDrawn = true;
                
            }
        }

        public void highlightSquares(List<Point> points)
        {
                for (int i = 0; i < points.Count; i++)
                {
                    bool potentialCheck = false;
                    int column = points[i].X; //Stores the X coordinate of the point to an integer variable
                    int row = points[i].Y;   //Stores the Y coordinate of the point to an integer variable
                    Piece piece1 = ChessBoard[previousRow, previousColumn]; //Stores the moving piece
                    Piece piece2 = ChessBoard[row, column]; //Stores what the moving piece will be replacing
                    ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * column), 5 + (60 * row), 50, 50);
                    ChessBoard[row, column] = ChessBoard[previousRow, previousColumn];
                    ChessBoard[previousRow, previousColumn] = null;
                    foreach (Piece piece in ChessBoard)
                    {
                        if (piece != null && piece is King king && king.IsWhite == turn)
                        {
                            potentialCheck = IsKingInCheck(king);
                            break; //Checks if the move would keep the king in check 
                        }
                    }
                    if (potentialCheck == false)
                    {
                        highlights.Add(new Rectangle(160 + 60 * column,  60 * row, 60, 60)); //finds the coordinates for the rectangle using the x and y of the points
                    }
                    ChessBoard[previousRow, previousColumn] = piece1;
                    ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * previousColumn), 5 + (60 * previousRow), 50, 50);
                    ChessBoard[row, column] = piece2;
                }
                if (highlights.Count == 0) //if no moves then checkmate
                {
                    Debug.WriteLine(turn ? "black has checkmated white" : "white has checkmated black");
                }
        }
        int previousRow = 0;
        int previousColumn = 0;
        //Will be used to store the row and column numbers for the piece that is going to move
        public void Update()
        {
            
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null && piece.IsWhite == turn)
                {
                    piece.Update(); //updates the pieces to check if they have been clicked

                    if (piece.movescalculated == true)
                    {
                        highlights.Clear();
                        previousRow = (piece.Position.Y - 5) / 60;  //calculates the row number for the pawn in the array using the coordinates of the rectangle
                        previousColumn = (piece.Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle
                        highlightSquares(piece.legalmoves); //if they have been clicked and legal moves were found, these moves are highlighted.
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

                             

                                foreach (Piece piece1 in ChessBoard) 
                                {
                                    if (piece1 is Pawn pawn)
                                    {
                                        pawn.movedtwoSquares = false;

                                    }
                                }

                                if (leftclickPressed == false && mouse.LeftButton == ButtonState.Pressed)
                                {
                                    leftclickPressed = true;
                                 
                                }
                                if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released) //If clicked
                                {
                                  
                                    leftclickPressed = false; //Sets to false for the next frame
                                    if (ChessBoard[previousRow, previousColumn] is Rook rook) //if the moving piece is a rook set has moved to true
                                    {
                                        rook.hasMoved = true;
                                    }
                                    else if (ChessBoard[previousRow, previousColumn] is King king) //if the moving piece is a king set has moved to true
                                    {
                                        king.hasMoved = true;
                                    }
                                    else if (ChessBoard[previousRow, previousColumn] is Pawn pawn && Math.Abs(previousRow - row) == 2 ) //Checks if the pawn is moving two squares
                                    {
                                        pawn.movedtwoSquares = true;
                                    }

                                

                                    if (ChessBoard[previousRow, previousColumn] is King && previousColumn - 2 == col) //Checks if the move is a queenside castle
                                    {
                                        ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                        ChessBoard[row, col] = ChessBoard[previousRow, previousColumn]; //Changes the position of the piece in the array after the move has been made
                                        ChessBoard[previousRow, previousColumn] = null; //previous position is empty
                                        ChessBoard[row, 0].Position = new Rectangle(165 + (60 * 3), 5 + (60 * row), 50, 50); //changes the position of the rook
                                        ChessBoard[row, 3] = ChessBoard[row, 0];
                                        ChessBoard[row, 0] = null;
                                        highlights.Clear(); //Clears the highlights because a move has been made
                                        highlightsDrawn = false; //Move has been made
                                        turn = !turn;
                                    }
                                    else if (ChessBoard[previousRow, previousColumn] is King && previousColumn + 2 == col) //Checks if the move is a kingside castle
                                    {

                                        ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                        ChessBoard[row, col] = ChessBoard[previousRow, previousColumn]; //Changes the position of the piece in the array after the move has been made
                                        ChessBoard[previousRow, previousColumn] = null; //previous position is empty
                                        ChessBoard[row, 7].Position = new Rectangle(165 + (60 * 5), 5 + (60 * row), 50, 50);
                                        ChessBoard[row, 5] = ChessBoard[row, 7];
                                        ChessBoard[row, 7] = null;
                                        highlights.Clear(); //Clears the highlights because a move has been made
                                        highlightsDrawn = false; //Move has been made
                                        turn = !turn;
                                    }
                                    else if (ChessBoard[previousRow, previousColumn] is Pawn pawnenPassant && Math.Abs(previousColumn - col) == 1 && ChessBoard[row, col] == null) //checks if the move is diagonal and if square is empty
                                    {
                                        if (pawnenPassant.IsWhite == true)
                                        {
                                                ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                                ChessBoard[row, col] = ChessBoard[previousRow, previousColumn]; //Changes the position of the piece in the array after the move has been made
                                                ChessBoard[previousRow, previousColumn] = null; //previous position is empty
                                                ChessBoard[row + 1, col] = null; //removes the pawn below the square
                                                highlights.Clear(); //Clears the highlights because a move has been made
                                                highlightsDrawn = false; //Move has been made
                                                turn = !turn;
                                            
                                        }
                                        else
                                        {
                                                ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                                ChessBoard[row, col] = ChessBoard[previousRow, previousColumn]; //Changes the position of the piece in the array after the move has been made
                                                ChessBoard[previousRow, previousColumn] = null; //previous position is empty
                                                ChessBoard[row - 1, col] = null; //removes the pawn below the square
                                                highlights.Clear(); //Clears the highlights because a move has been made
                                                highlightsDrawn = false; //Move has been made
                                                turn = !turn;              
                                        }
                                    }
                                    else
                                    {
                                      
                                        ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                        ChessBoard[row, col] = ChessBoard[previousRow, previousColumn]; //Changes the position of the piece in the array after the move has been made
                                        ChessBoard[previousRow, previousColumn] = null; //previous position is empty
                                        highlights.Clear(); //Clears the highlights because a move has been made
                                        highlightsDrawn = false; //Move has been made
                                        turn = !turn;
                                    }
                                   
                                    if (check == true)
                                    {
                                        check = false; //if a move has been made while the board is in check it would be a move that stops the check
                                    }

                                    foreach (Piece piece1 in ChessBoard)
                                    {
                                        if (piece1 is King king1)
                                        {
                                           if (king1.IsWhite == turn)
                                           {
                                                check = IsKingInCheck(king1);
                                           }
                                     
                                        }


                                    }
                                    break; //Stops the search


                                }
                            }
                        }

                    } 




                }
                
            }

            
        }
        public bool IsKingInCheck(King king)
        {
            int row = (king.Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
            int col = (king.Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle

            for (int i = 1; i <= 7; i++) //Checks the squares to the right of the king
            {

                if (col + i < 8 && ChessBoard[row, col + i] != null)
                {

                    if (ChessBoard[row, col + i].IsWhite != king.IsWhite && (ChessBoard[row, col + i] is Rook || ChessBoard[row, col + i] is Queen))
                    {  //if the square that is being checked has an enemy rook/queen the king is in check
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");    
                        return true;
                      
                    }
                    else
                    {
                        break;
                    }



                }
            }
            for (int i = 1; i <= 7; i++) //Checks the squares below the king
            {
                if (row + i < 8 && ChessBoard[row + i, col] != null)
                {
                        
                    if (ChessBoard[row + i, col].IsWhite != king.IsWhite && ChessBoard[row + i, col] is Rook || ChessBoard[row + i, col].IsWhite != king.IsWhite && ChessBoard[row + i, col] is Queen)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }



                }
            }
            for (int i = 1; i <= 7; i++) //Checks the squares to the left of the king
            {
                if (col - i >= 0 && ChessBoard[row, col - i] != null)
                {

                    if (ChessBoard[row, col - i].IsWhite != king.IsWhite && ChessBoard[row, col - i] is Rook || ChessBoard[row, col - i].IsWhite != king.IsWhite && ChessBoard[row, col - i] is Queen)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }



                }
            }
            for (int i = 1; i <= 7; i++) //Checks the squares above the king
            {
                if (row - i >= 0 && ChessBoard[row - i, col] != null)
                {


                    if (ChessBoard[row - i, col].IsWhite != king.IsWhite && ChessBoard[row - i, col] is Rook || ChessBoard[row - i, col].IsWhite != king.IsWhite && ChessBoard[row - i, col] is Queen)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }



                }
            }
            //checks for knights
            for (int j = -1; j <= 1; j = j + 2)
            {
                for (int i = 2; i >= -2; i = i - 4)
                {
                    if (row + i >= 0 && row + i < 8 && col + j >= 0 && col + j < 8 && ChessBoard[row + i, col + j] != null) //checks if not out of bounds
                    {

                        if (ChessBoard[row + i, col + j].IsWhite != king.IsWhite && ChessBoard[row + i, col + j] is Knight) //checks if the square has a knight
                        {
                            Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                            return true;
                        }




                    }

                    if (col + i >= 0 && col + i < 8 && row + j >= 0 && row + j < 8 && ChessBoard[row + j, col + i] != null)
                    {

                        if (ChessBoard[row + j, col + i].IsWhite != king.IsWhite && ChessBoard[row + j, col + i] is Knight)
                        {
                            Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                            return true;
                        }



                    }
                }
            }
            //CHECKING FOR BISHOP OR QUEEN
            for (int i = 1; i <= 7; i++) //Checks top right diagonal
            {
                if (row - i >= 0 && col + i < 8 && ChessBoard[row - i, col + i] != null)
                {
                    if (ChessBoard[row - i, col + i].IsWhite != king.IsWhite && ChessBoard[row - i, col + i] is Bishop || ChessBoard[row - i, col + i].IsWhite != king.IsWhite && ChessBoard[row - i, col + i] is Queen) //Checks if square has no piece on it
                    {
                       
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }

                }
            }

            for (int i = 1; i <= 7; i++) //Checks bottom right diagonal
            {
                if (row + i < 8 && col + i < 8 && ChessBoard[row + i, col + i] != null)
                {
                    if (ChessBoard[row + i, col + i].IsWhite != king.IsWhite && ChessBoard[row + i, col + i] is Bishop || ChessBoard[row + i, col + i].IsWhite != king.IsWhite && ChessBoard[row + i, col + i] is Queen) //Checks if square has no piece on it
                    {
                      
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            for (int i = 1; i <= 7; i++) //Checks top left diagonal
            {
                if (row - i >= 0 && col - i >= 0 && ChessBoard[row - i, col - i] != null)
                {
                    if (ChessBoard[row - i, col - i].IsWhite != king.IsWhite && ChessBoard[row - i, col - i] is Bishop || ChessBoard[row - i, col - i].IsWhite != king.IsWhite && ChessBoard[row - i, col - i] is Queen) //Checks if square has no piece on it
                    {
                       
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            for (int i = 1; i <= 7; i++) //Checks bottom left diagonal
            {
                if (row + i < 8 && col - i >= 0 && ChessBoard[row + i, col - i] != null)
                {
                    if (ChessBoard[row + i, col - i].IsWhite != king.IsWhite && ChessBoard[row + i, col - i] is Bishop || ChessBoard[row + i, col - i].IsWhite != king.IsWhite && ChessBoard[row + i, col - i] is Queen) //Checks if square has no piece on it
                    {
                        
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            //Checking for pawns
            if (king.IsWhite == true)
            {
                if (row - 1 >= 0 && col + 1 < 8)
                {
                    if (ChessBoard[row - 1, col + 1] != null && ChessBoard[row - 1, col + 1].IsWhite != king.IsWhite && ChessBoard[row - 1, col + 1] is Pawn)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                }
                if (row - 1 >= 0 && col - 1 >= 0)
                {
                    if (ChessBoard[row - 1, col - 1] != null && ChessBoard[row - 1, col - 1].IsWhite != king.IsWhite && ChessBoard[row - 1, col - 1] is Pawn)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                }


            }
            else //if black king
            {
                if (row + 1 < 8 && col + 1 < 8)
                {
                    if (ChessBoard[row + 1, col + 1] != null && ChessBoard[row + 1, col + 1].IsWhite != king.IsWhite && ChessBoard[row + 1, col + 1] is Pawn)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                }
                if (row + 1 < 8 && col - 1 >= 0)
                {
                    if (ChessBoard[row + 1, col - 1] != null && ChessBoard[row + 1, col - 1].IsWhite != king.IsWhite && ChessBoard[row + 1, col - 1] is Pawn)
                    {
                        Debug.WriteLine(king.IsWhite ? "white king is in check" : "black king is in check");
                        return true;
                    }
                }

            }
            Debug.WriteLine("King is not in check");
            return false;
        }
    }
}
