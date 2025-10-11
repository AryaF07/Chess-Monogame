using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
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
            public Piece capturedPiece;
            public Move(Piece piece1, int nextcol1, int nextrow1, int prevcol1, int prevrow1, Piece capture)
            {
                piece = piece1;
                nextcol = nextcol1;
                nextrow = nextrow1;
                prevcol = prevcol1;
                prevrow = prevrow1; 
                capturedPiece = capture;
            }
        }


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

        public int evaluate()
        {
            int evaluation = 0;
            foreach (Piece piece in board.ChessBoard)
            {
                if (piece != null)
                {
                    evaluation += piece.evaluate();
                    evaluation += piece.pieceValue;
                }

               
            }
            return -evaluation;
        }



        public List<Move> findmoves(bool colour)
        {
            List<Move> movingpieces = new List<Move>();

            foreach (Piece piece in board.ChessBoard)
            {
                if (piece != null && piece.IsWhite == colour)
                {
                    piece.findMoves();
                   
                        foreach (Point point in piece.legalmoves)
                        {
                            if (piece is null)
                        {
                            Debug.WriteLine("nul");
                        }

                            //point.x = nextcol point.y = nextrow  (piece.Position.X - 165) / 60 = prevcol (piece.Position.Y - 5) / 60 = prevrow
                            int prevcol = (piece.Position.X - 165) / 60;
                            int prevrow = (piece.Position.Y - 5) / 60;
                            Piece previouspiece = board.ChessBoard[point.Y, point.X];
                            board.ChessBoard[point.Y, point.X] = piece;
                            board.ChessBoard[(piece.Position.Y - 5) / 60, (piece.Position.X - 165) / 60] = null;
                            piece.Position = new Rectangle(165 + (60 * point.X), 5 + (60 * point.Y), 50, 50);

                            if (board.IsKingInCheck(colour) == false && previouspiece is not King)
                            {
                                movingpieces.Add(new Move(piece, point.X, point.Y, prevcol, prevrow, previouspiece));
                              
                            }
                            board.ChessBoard[(piece.Position.Y - 5) / 60, (piece.Position.X - 165) / 60] = piece;
                            board.ChessBoard[point.Y, point.X] = previouspiece;
                            piece.Position = new Rectangle(165 + (60 * prevcol), 5 + (60 * prevrow), 50, 50);


                        }
                    

                    piece.legalmoves.Clear();
                }
               
            }

            return movingpieces;
        }

        public void move()
        {
            Move move = Minimax(board, 3).move;
            Debug.WriteLine("minimax complete");
            makeMove(board,move);
        }

        public void makeMove(Board board, Move move)
        {
            if (move.piece != null)
            {
                
                move.capturedPiece = board.ChessBoard[move.nextrow, move.nextcol];
                board.ChessBoard[move.nextrow, move.nextcol] = move.piece;
                board.ChessBoard[move.prevrow, move.prevcol] = null;
                move.piece.Position = new Rectangle(165 + (60 * move.nextcol), 5 + (60 * move.nextrow), 50, 50);
                board.turn = !board.turn;
            }

        }

        public void undoMove(Board board, Move move)
        {
            board.ChessBoard[move.prevrow, move.prevcol] = move.piece;
            board.ChessBoard[move.nextrow, move.nextcol] = move.capturedPiece;
            move.piece.Position = new Rectangle(165 + (60 * move.prevcol), 5 + (60 * move.prevrow), 50, 50);
            board.turn = !board.turn;
        }


        (int evaluation,Move move) Minimax(Board board,int depth)
        {
  
            if (board.checkmate == true || depth == 0)
            {
                return (evaluate(),new Move(null,0,0,0,0,null));

            }
            Move bestMove = new Move(null,0,0,0,0,null);


            
            Debug.WriteLine("Move made");

            if (board.turn == false)//maximising player
            {
                int maxEval = int.MinValue;
                List<Move> movingpieces = findmoves(false);
                foreach (Move move in movingpieces)
                {
                    makeMove(board, move);
                    int eval = Minimax(board, depth - 1).evaluation;
                    undoMove(board, move);
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = move;
                    }
                }
                return (maxEval,bestMove);


            }
            else // minimising player (bot)
            {
                int minEval = int.MaxValue;
                List<Move> movingpieces = findmoves(true);
                foreach (Move move in movingpieces)
                {
                    makeMove(board, move);
                    int eval = Minimax(board, depth - 1).evaluation;
                    undoMove(board, move);
                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestMove = move;
                    }
                }
                return (minEval,bestMove);
            }

        }

    }
}
