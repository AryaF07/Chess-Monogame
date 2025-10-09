using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace ChessNEA
{
    public class Bot
    {
        public  struct Move //This is where "moves" will be stored during the search process in the Minimax algorithm
        {
            public Piece piece;
            public int nextcol;
            public int nextrow;
            public int prevcol;
            public int prevrow;
            public Move(Piece piece1, int nextcol1, int nextrow1, int prevcol1, int prevrow1)
            {
                piece = piece1;
                nextcol = nextcol1;
                nextrow = nextrow1;
                prevcol = prevcol1;
                prevrow = prevrow1; 
            }
        }

        protected static Board board;
        public int previousCol;
        public int previousRow;
        public int col;
        public int row;
        Move bestMove = new Move();

        Random rand = new Random();
        public static void setBoard(Board _board)
        {
            board = _board; // board attribute will equal whatever parameter is passed into the function
        }
  
        public int evaluate()
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
            return evaluation;
        }

        public List<Move> findmoves()
        {
            List<Move> movingpieces = new List<Move>();

            foreach (Piece piece in board.ChessBoard)
            {
                if (piece != null)
                {
                    piece.findMoves();
                    foreach (Point point in piece.legalmoves)
                    {
                        movingpieces.Add(new Move(piece, point.X, point.Y, (piece.Position.Y-5)/60, (piece.Position.Y - 5) / 60));
                    }
                }
                piece.legalmoves.Clear();
            }

            return movingpieces;
        }

        public void move(bool colour)
        {

           List<Piece> movePieces = new List<Piece>();
            foreach (Piece piece in board.ChessBoard)
            {
                if (piece != null && piece.IsWhite == colour)
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
        }

        public void makeMove(Board board, Move move)
        {
            move.piece.Position = new Rectangle(165 + (60 * move.nextcol), 5 + (60 * move.nextrow), 50, 50);
            board.ChessBoard[move.nextrow, move.nextcol] = move.piece;
            board.ChessBoard[move.prevrow, move.prevcol] = null;
        }


        public int Minimax(Board board,int depth, int maxEval, int minEval)
        {
            List<Move> movingpieces = findmoves();

            if (board.checkmate == true || depth == 0)
            {
                return evaluate();
            }

            if (board.turn == true)//minimising player
            {
                minEval = 999999999;
                move(true);

            }
            else // maximising player (bot)
            {
                maxEval = -999999999;
            }
        }

    }
}
