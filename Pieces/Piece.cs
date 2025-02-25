using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ChessNEA
{
    public abstract class Piece 
    { //Piece will never be instantiated 
        // A chess board is an array of chess pieces so every chess piece will inherit the piece class
        public bool IsWhite; //Determines whether a piece is white or not, allowing for correct sprites to be loaded
        public Texture2D pieceSprite; //Holds the sprite of the chess piece
        public Rectangle Position; //Allows me to change the location (x,y) and width and height of sprite.
        public bool leftclickPressed; // Indicates whether the left click has pressed the sprite (true if pressed, false otherwise).
        protected static Board board; //this references the instance of the board for which the piece will interact with, allowing the piece to access the array to implement logic
        public List<Point> legalmoves = new List<Point>(); //this list will contain all of the legal moves that are calculated for a piece
        public bool movescalculated; //determines whether legal moves have been found for a piece.
       
        public static void setBoard(Board _board)
        {
            board = _board; // board attribute will equal whatever parameter is passed into the function.

        }

        public abstract void findMoves();
        public abstract void LoadContent(ContentManager content);
        //Allows for every class that inherits from piece to load in their specific sprites

        public abstract void Draw(SpriteBatch spriteBatch);
        //Allows for every class that inherits from piece to draw in their specific sprites
        public abstract void Update();
        //Allows for every class that inherits from piece to update the state of the piece every update cycle


    }
}
