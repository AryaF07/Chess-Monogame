﻿using System;
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
    class King : Piece
    {
        public King(Board board, bool iswhite, Rectangle position) 
        {
            IsWhite = iswhite;
            Position = position;
        }

        public override void LoadContent(ContentManager content)
        {
            if (IsWhite)
            {
                pieceSprite = content.Load<Texture2D>("KingW");
            }
            else
            {
                pieceSprite = content.Load<Texture2D>("KingB");
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
                Debug.WriteLine(IsWhite ? "Mouse has clicked white king" : "Mouse has clicked black king");
                //Displays line depending on if the piece is white or not.
                findMoves();
            }
        }

        void findMoves()
        {
            legalmoves.Clear();
            int row = (Position.Y - 5) / 60; //calculates the row number for the pawn in the array using the coordinates of the rectangle
            int col = (Position.X - 165) / 60; //calculates the column number for the pawn in the array using the coordinates of the rectangle

            for (int i = -1; i <= 1; i++)
            {
                if (row + i >= 0 && row + i < 8 && col - 1 >= 0)
                {
                    if (board.ChessBoard[row + i, col - 1] == null || board.ChessBoard[row + i, col - 1].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col - 1, row + i));
                    }
                    if (board.ChessBoard[row + i, col] == null || board.ChessBoard[row + i, col].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col, row + i));
                    }
                }
                if (row + i >= 0 && row + i < 8 && col + 1 <8)
                {
                    if (board.ChessBoard[row + i, col + 1] == null || board.ChessBoard[row + i, col + 1].IsWhite != this.IsWhite)
                    {
                        legalmoves.Add(new Point(col + 1, row + i));
                    }
                }
            }


            movescalculated = true;
        }
    }
}
