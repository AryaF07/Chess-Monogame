using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace ChessNEA
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Board board;
        
        //Board is instantiated as the game begins.
        Texture2D chessboardSprite;
        // holds the chess board sprite

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            board = new Board();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
          
           
        }

        protected override void LoadContent()
        {
            ContentManager content = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            board.LoadContent(content);
            //Loads all the piece sprites
            chessboardSprite = Content.Load<Texture2D>("Board");
            //Loads the sprite for the chess board
 
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
                board.Update(gameTime);
            if (board.promoted == true)
            {
                board.LoadContent(Content);
                board.promoted = false;
            }
            //Updates each chess piece in the chessboard array
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);
            _spriteBatch.Begin();

            _spriteBatch.Draw(chessboardSprite, new Rectangle(160,0,480,480),Color.White);
            //Draws the chess board sprite
            board.Draw(_spriteBatch);
            //Loads all the piece sprites
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
