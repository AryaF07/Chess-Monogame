using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
namespace ChessNEA
{
    public class Bot
    {
        protected static Board board;
        public int previousCol;
        public int previousRow;
        public int col;
        public int row;
        Random rand = new Random();
        public static void setBoard(Board _board)
        {
            board = _board; // board attribute will equal whatever parameter is passed into the function
        }

        public void evaluate()
        {
            int evaluation = 0;
            foreach(Piece piece in board.ChessBoard)
            {
                if (piece!=null)
                {
                    evaluation += piece.evaluate();
                    evaluation += piece.pieceValue;
                }
            }
            Debug.WriteLine(evaluation);
        }

        public void move()
        {

           List<Piece> movePieces = new List<Piece>();
            foreach (Piece piece in board.ChessBoard)
            {
                if (piece != null && piece.IsWhite == false)
                {
                    piece.botPiece = true;
                    piece.findMoves();
                    if (piece.legalmoves.Count > 0)
                    {
                        movePieces.Add(piece);
                    }
                }
                
            }
            int randomNum = rand.Next(movePieces.Count);

            int randomNum2 = rand.Next(movePieces[randomNum].legalmoves.Count);
            previousRow = (movePieces[randomNum].Position.Y - 5) / 60;
            previousCol = (movePieces[randomNum].Position.X - 165) / 60;
            
            col = movePieces[randomNum].legalmoves[randomNum2].X;
            row = movePieces[randomNum].legalmoves[randomNum2].Y;
            foreach (Piece piece in movePieces)
            {
                piece.legalmoves.Clear();
            }

            
            Debug.WriteLine("Move made");
        }

    }
}
