using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ChessNEA
{
    class Bot
    {
        protected static Board board;
     

        public static void setBoard(Board _board)
        {
            board = _board; // board attribute will equal whatever parameter is passed into the function
        }

        public void Evaluation()
        {
            int evaluation = 0;
            foreach(Piece piece in board.ChessBoard)
            {
                if (piece!=null)
                {
                    evaluation += piece.evaluate();
                }
            }
            Debug.WriteLine(evaluation);
        }
    }
}
