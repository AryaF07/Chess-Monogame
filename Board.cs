using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ChessNEA
{
    public class Board
    {
        public Piece[,] ChessBoard;
        Texture2D highlightSprite;
        Texture2D whitewinscreen;
        Texture2D blackwinscreen;
        Texture2D stalematescreen;
        Texture2D fiftymoverulescreen;
        Texture2D queenPromotionW;
        Texture2D knightPromotionW;
        Texture2D rookPromotionW;
        Texture2D bishopPromotionW;
        Texture2D queenPromotionB;
        Texture2D knightPromotionB;
        Texture2D rookPromotionB;
        Texture2D bishopPromotionB;
        Texture2D insufficientmaterialscreen;
        SpriteFont Font; //Stores font for timer text

        List<Rectangle> highlights = new List<Rectangle>();//this list will contain the rectangles for the move highlights
        Rectangle [] promotions = new Rectangle[4];//this array will contain the rectangles for the promotion sprites

        double duration = 600 ; //Stores how much time each player has
        double elapsedtimeW; //Stores how much time has elapsed during a whites turn
        double elapsedtimeB;//Stores how much time has elapsed during a whites turn

        string timerB = "10:00"; //timer for black
        string timerW = "10:00"; //timer for white

        bool highlightsDrawn;
        bool leftclickPressed;
        private bool turn = true; //true = whites turn false = blacks turn
        public bool check = false;
        bool checkmate = false;
        bool stalemate = false;
        bool promotewhite = false;
        bool promoteblack = false;
        bool insufficientmaterial = false;
        public bool promoted = true;
        bool pawnmoved = false; //indicates if a pawn has moved at any time
        bool piececaptured = false; //indicates if a piece has been captured at any time
        int numberofmoves = 0; //will keep count of how many moves have been made

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


            ChessBoard[0, 1] = new Knight(this, false, new Rectangle(225, 5, 50, 50));
            ChessBoard[0, 2] = new Bishop(this, false, new Rectangle(285, 5, 50, 50));

            ChessBoard[0, 4] = new King(this, false, new Rectangle(405, 5, 50, 50));
            ChessBoard[0, 6] = new Knight(this, false, new Rectangle(525, 5, 50, 50));
            ChessBoard[0, 7] = new Rook(this, false, new Rectangle(585, 5, 50, 50));
            //black pieces


       
            ChessBoard[7, 2] = new Bishop(this, true, new Rectangle(285, 425, 50, 50));

            ChessBoard[7, 4] = new King(this, true, new Rectangle(405, 425, 50, 50));
            ChessBoard[7, 5] = new Bishop(this, true, new Rectangle(465, 425, 50, 50));
     
            

            //white pieces

            // This puts each chess piece in their positions same as a regular chess board



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
            whitewinscreen = content.Load<Texture2D>("whitewin");
            blackwinscreen = content.Load<Texture2D>("blackwin");
            stalematescreen = content.Load<Texture2D>("stalemate");
            fiftymoverulescreen = content.Load<Texture2D>("50moverule");
            insufficientmaterialscreen = content.Load<Texture2D>("insufficientmaterial");

            queenPromotionW =  content.Load<Texture2D>("QueenW");
            knightPromotionW = content.Load<Texture2D>("KnightW");
            rookPromotionW = content.Load<Texture2D>("RookW");
            bishopPromotionW = content.Load<Texture2D>("BishopW");

            queenPromotionB = content.Load<Texture2D>("QueenB");
            knightPromotionB = content.Load<Texture2D>("KnightB");
            rookPromotionB = content.Load<Texture2D>("RookB");
            bishopPromotionB = content.Load<Texture2D>("BishopB");

            Font = content.Load<SpriteFont>("File");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle gameScreen = new Rectangle(125, 150, 564, 186);
         
            if (turn == true)
            {
                double remainingTime = Math.Max(0, duration - elapsedtimeW); //stores how much time is left
                int minutes = (int)(remainingTime / 60);
                int seconds = (int)(remainingTime - (minutes * 60));
                timerW = $"{minutes:D2}:{seconds:D2}"; //puts time in MM:SS format
            }
            else
            {
                double remainingTime = Math.Max(0, duration - elapsedtimeB); //stores how much time is left
                int minutes = (int)(remainingTime / 60);
                int seconds = (int)(remainingTime - (minutes * 60));
                timerB = $"{minutes:D2}:{seconds:D2}";//puts time in MM:SS format
            }
            spriteBatch.DrawString(Font, timerB,new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(Font, timerW, new Vector2(50, 400), Color.White);
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
           
            if (checkmate == true) //Displays win screen if a player has been checkmated
            {

                if (turn == true)
                {
                    spriteBatch.Draw(blackwinscreen, gameScreen, Color.White);
                }
                else
                {
                    spriteBatch.Draw(whitewinscreen, gameScreen, Color.White);
                }

            }
            if (stalemate == true)
            {
                spriteBatch.Draw(stalematescreen, gameScreen, Color.White);
            }
            if (numberofmoves == 50 && pawnmoved == false && piececaptured == false) //50 move rule
            {
                spriteBatch.Draw(fiftymoverulescreen, gameScreen, Color.White);
            }
            if (insufficientmaterial == true)
            {
                spriteBatch.Draw(insufficientmaterialscreen, gameScreen, Color.White);
            }
            if (elapsedtimeB == duration) //if black runs out of time white wins
            {
                spriteBatch.Draw(whitewinscreen, gameScreen, Color.White);
            }
            else if (elapsedtimeW == duration) //if white runs out of time black wins 
            {
                spriteBatch.Draw(blackwinscreen, gameScreen, Color.White);
            }
            if (promotewhite == true)
            {
                
                Texture2D[] promotionTextures = { queenPromotionW, rookPromotionW, knightPromotionW, bishopPromotionW }; //Creates array for sprites to promote
                for (int i = 0; i < 4; i++)
                {
                    promotions[i] = new Rectangle(220 + (i * 100), 200, 80, 80);
                    spriteBatch.Draw(promotionTextures[i], promotions[i], Color.White);
                }
               

            }
            else if (promoteblack == true)
            {
                spriteBatch.Draw(highlightSprite, new Rectangle(125, 150, 564, 186), Color.Black);
                Texture2D[] promotionTextures = { queenPromotionB, rookPromotionB, knightPromotionB, bishopPromotionB }; //Creates array for sprites to promote
                for (int i = 0; i < 4; i++)
                {
                    promotions[i] = new Rectangle(220 + (i * 100), 200, 80, 80);
                    spriteBatch.Draw(promotionTextures[i], promotions[i], Color.White);
                }
                
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
        }
        int previousRow = 0;
        int previousColumn = 0;
        //Will be used to store the row and column numbers for the piece that is going to move
        public void Update(GameTime gameTime)
        {
            if (promotewhite == true)
            {
                PromotePawn(true);
            }
            else if (promoteblack == true)
            {
                PromotePawn(false);
            }

            if (turn == true) //during whites turn
            {
                elapsedtimeW += gameTime.ElapsedGameTime.TotalSeconds; //Adds on time elapsed from last frame
                if (elapsedtimeW >= duration)
                {
                    elapsedtimeW = duration; //only the duration should have elapsed
                }
            }
            else //blacks turn
            {
                elapsedtimeB += gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedtimeB >= duration)
                {
                    elapsedtimeB = duration;
                }
            }

            foreach (Piece piece in ChessBoard)
            {
                if (piece != null && piece.IsWhite == turn && promotewhite == false && promoteblack == false)
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

                                if (numberofmoves == 50) //resets when reach 50
                                {
                                    numberofmoves = 0;
                                    piececaptured = false;
                                    pawnmoved = false;
                                }


                                

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
                                                piececaptured = true;
                                                pawnmoved = true;
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
                                                piececaptured = true;
                                                pawnmoved = true;
                                        }
                                    }
                                    else
                                    {
                                      if (ChessBoard[previousRow, previousColumn] is Pawn)
                                      {
                                         pawnmoved =true;
                                      }
                                        ChessBoard[previousRow, previousColumn].Position = new Rectangle(165 + (60 * col), 5 + (60 * row), 50, 50); //Changes the X and Y coordinates of the rectangle for the piece
                                        if (ChessBoard[row, col] != null) //Checks if it will be a capture
                                        {
                                            piececaptured = true;
                                        }
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
                                    numberofmoves++; //Number of moves increases after move has been made
                                    foreach (Piece piece1 in ChessBoard)
                                    {
                                        if (piece1 is King king1)
                                        {
                                           if (king1.IsWhite == turn)
                                           {
                                                check = IsKingInCheck(king1);
                                                if (check == true)
                                                {
                                                    checkmate = Checkmate(king1);
                                                }
                                                else
                                                {
                                                    stalemate = Checkmate(king1);
                                                }
                                           }
                                        }
                                    }
                                    insufficientmaterial = InsufficientMaterial();
                                    if (ChessBoard[row, col] != null && ChessBoard[row, col] is Pawn && ChessBoard[row, col].IsWhite == true && row == 0) //Checks for white pawn on first row
                                    {
                                        promotewhite = true;
                                    }
                                    else if (ChessBoard[row, col] != null &&  ChessBoard[row, col] is Pawn && ChessBoard[row, col].IsWhite == false && row == 7)//Checks for black pawn on last row
                                    {
                                        promoteblack = true;
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
                            return true;
                        }
                    }

                    if (col + i >= 0 && col + i < 8 && row + j >= 0 && row + j < 8 && ChessBoard[row + j, col + i] != null)
                    {

                        if (ChessBoard[row + j, col + i].IsWhite != king.IsWhite && ChessBoard[row + j, col + i] is Knight)
                        {
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
                        return true;
                    }
                }
                if (row - 1 >= 0 && col - 1 >= 0)
                {
                    if (ChessBoard[row - 1, col - 1] != null && ChessBoard[row - 1, col - 1].IsWhite != king.IsWhite && ChessBoard[row - 1, col - 1] is Pawn)
                    {
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
                        return true;
                    }
                }
                if (row + 1 < 8 && col - 1 >= 0)
                {
                    if (ChessBoard[row + 1, col - 1] != null && ChessBoard[row + 1, col - 1].IsWhite != king.IsWhite && ChessBoard[row + 1, col - 1] is Pawn)
                    {
                        return true;
                    }
                }

            }    
            return false;
        }

        bool Checkmate(King king) //checks if the king is in checkmate
        {
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null && piece.IsWhite == turn)
                {
                    piece.findMoves();
                    piece.movescalculated = false;
                    for (int i = 0; i < piece.legalmoves.Count; i++)
                    {
                        bool potentialCheck = false;
                        int column = piece.legalmoves[i].X; //Stores the X coordinate of the point to an integer variable
                        int row = piece.legalmoves[i].Y;   //Stores the Y coordinate of the point to an integer variable
                        int prevrow = (piece.Position.Y - 5) / 60;
                        int prevcol = (piece.Position.X - 165) / 60;
                        Piece piece1 = ChessBoard[prevrow, prevcol]; //Stores the moving piece
                        Piece piece2 = ChessBoard[row, column]; //Stores what the moving piece will be replacing
                        ChessBoard[prevrow, prevcol].Position = new Rectangle(165 + (60 * column), 5 + (60 * row), 50, 50);
                        ChessBoard[row, column] = ChessBoard[prevrow, prevcol];
                        ChessBoard[prevrow, prevcol] = null;
                        potentialCheck = IsKingInCheck(king);
                        ChessBoard[prevrow, prevcol] = piece1;
                        ChessBoard[prevrow, prevcol].Position = new Rectangle(165 + (60 * prevcol), 5 + (60 * prevrow), 50, 50);
                        ChessBoard[row, column] = piece2;
                        if (potentialCheck == false)
                        {
                            piece.legalmoves.Clear();
                            return false;
                        }
                    }
                     piece.legalmoves.Clear();
                }
            }
            return true;
        }
        void PromotePawn(bool IsWhite)
        {
            foreach (Rectangle rect in promotions)
            {
                MouseState mouse = Mouse.GetState();
                Rectangle mouse2 = new Rectangle(mouse.X, mouse.Y, 1, 1);
                if (mouse2.Intersects(rect)) //Checks if mouse is clicking the promotion icons
                {      
                    if (leftclickPressed == false && mouse.LeftButton == ButtonState.Pressed)
                    {
                        leftclickPressed = true;
                    }
                    if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released) //If clicked
                    {
                        leftclickPressed = false;

                        int pawnColumn = 0;
                        if (IsWhite == true)
                        {
                            for (int i = 0; i < 8; i++) //Finds column number of white pawn
                            {
                                if (ChessBoard[0, i] is Pawn && ChessBoard[0, i].IsWhite == IsWhite) //if element is a pawn
                                {
                                    pawnColumn = i;
                                    break;
                                }
                            }

                            if (rect == promotions[0]) //Queen rectangle is at index 0
                            {
                                ChessBoard[0, pawnColumn] = new Queen(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 5, 50, 50)); //replaces pawn with queen
                                promotewhite = false;
                                promoted = true;
                                   
                            }
                            else if (rect == promotions[1]) //Rook rectangle at index 1 
                            {
                                ChessBoard[0, pawnColumn] = new Rook(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 5, 50, 50));
                                promotewhite = false;
                                promoted = true;
                            }
                            else if (rect == promotions[2])//Knight rectangle at index 2 
                            {
                                ChessBoard[0, pawnColumn] = new Knight(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 5, 50, 50));
                                promotewhite = false;
                                promoted = true;
                            }
                            else if (rect == promotions[3])//Bishop rectangle at index 3 
                            {
                                ChessBoard[0, pawnColumn] = new Bishop(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 5, 50, 50));
                                promotewhite = false;
                                promoted = true;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++) //Finds column number of black pawn
                            {
                                if (ChessBoard[7, i] is Pawn && ChessBoard[7, i].IsWhite == IsWhite) //if element is a pawn
                                {
                                    pawnColumn = i;
                                    break;
                                }
                            }
                            if (rect == promotions[0]) //Queen rectangle is at index 0
                            {
                                ChessBoard[7, pawnColumn] = new Queen(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 425, 50, 50));
                                promoteblack = false;
                                promoted = true;
                            }
                            else if (rect == promotions[1])//Rook rectangle at index 1 
                            {
                                ChessBoard[7, pawnColumn] = new Rook(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 425, 50, 50));
                                promoteblack = false;
                                promoted = true;
                            }
                            else if (rect == promotions[2])//Knight rectangle at index 2 
                            {
                                ChessBoard[7, pawnColumn] = new Knight(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 425, 50, 50));
                                promoteblack = false;
                                promoted = true;
                            }
                            else if (rect == promotions[3])//Bishop rectangle at index 3
                            {
                                ChessBoard[7, pawnColumn] = new Bishop(this, IsWhite, new Rectangle(165 + (60 * pawnColumn), 425, 50, 50));
                                promoteblack = false;
                                promoted = true;
                            }
                        }
                    }
                }
            } 
        }

        bool InsufficientMaterial()
        {
            int bishopwhitesquareW = 0; //White bishops on white squares
            int bishopblacksquareW = 0; //White bishops on black squares
            int bishopwhitesquareB = 0; //Black bishops on white squares
            int bishopblacksquareB = 0;//Black bishops on black squares
            int pawns = 0; //Stores number of pawns
            int blockedpawns = 0; //Stores number of pawns that cannot move due to being blocked
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null)
                {
                    if (piece is Rook or Queen)
                    {
                        return false;
                    }
                    else if (piece is Pawn)
                    {
                        pawns++;
                    }
                    else if (piece is Bishop)
                    {
                        if (piece.IsWhite == true && (((piece.Position.Y - 5) / 60) + ((piece.Position.X - 165) / 60)) % 2 == 0) //Adds the rows and columns and checks if its even
                        {
                            bishopwhitesquareW++;
                        }
                        else if (piece.IsWhite == true && (((piece.Position.Y - 5) / 60) + ((piece.Position.X - 165) / 60)) % 2 != 0)
                        {
                            bishopblacksquareW++;
                        }
                        else if (piece.IsWhite == false && (((piece.Position.Y - 5) / 60) + ((piece.Position.X - 165) / 60)) % 2 == 0) //Adds the rows and columns and checks if its even
                        {
                            bishopwhitesquareB++;
                        }
                        else if (piece.IsWhite == false && (((piece.Position.Y - 5) / 60) + ((piece.Position.X - 165) / 60)) % 2 != 0)
                        {
                            bishopblacksquareB++;
                        }
                    }
                }
            }
            foreach (Piece piece in ChessBoard)
            {
                if (piece != null  && ((piece.Position.Y - 5) / 60) - 1>= 0) //checks if index is valid
                {
                    if (ChessBoard[((piece.Position.Y - 5) / 60) - 1, (piece.Position.X - 165) / 60] != null)
                    {
                        if (piece is Pawn && piece.IsWhite == true && ChessBoard[((piece.Position.Y - 5) / 60) - 1, (piece.Position.X - 165) / 60].IsWhite == false && ChessBoard[((piece.Position.Y - 5) / 60) - 1, (piece.Position.X - 165) / 60] is Pawn) //checks if there is a pawn infront of the pawn
                        {
                            blockedpawns += 2; //because the pawn infront of the pawn would also be blocked
                        }

                    }

                }

            }
            if (blockedpawns < pawns) //checks if there are pawns that arent blocked
            {
                return false;
            }
            if ((bishopwhitesquareW == 0 || bishopblacksquareW == 0) && (bishopwhitesquareB == 0 || bishopblacksquareB == 0))
            {
                return true;
            }
           
            return false;
        }
    }
}
