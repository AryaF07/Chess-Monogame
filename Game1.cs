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
        bool screen1 = true;
        bool screen2 = false;
        bool startGame = false;
        bool leftclickPressed;
        Board board;
        SpriteFont Title; //font used in menus
        //Board is instantiated as the game begins.
        Texture2D chessboardSprite; // holds the chess board sprite
        Texture2D playButton;
        Texture2D endgameButton;

        Texture2D oneminute;
        Texture2D threeminutes;
        Texture2D tenminutes;
        Texture2D stopwatch;
        Rectangle[] minuteButtons = { new Rectangle(550, 25, 200, 100), new Rectangle(550, 175, 200, 100), new Rectangle(550, 325, 200, 100)}; //stores rectangles for buttons

        Rectangle playbutton = new Rectangle(300, 300, 200, 100);
        Rectangle endgamebutton = new Rectangle(670, 25, 100, 50);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            board = new Board();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            Title = Content.Load<SpriteFont>("title");
            playButton = Content.Load<Texture2D>("playbutton");
            endgameButton = Content.Load<Texture2D>("endgame");
            oneminute = Content.Load<Texture2D>("1minute");
            threeminutes = Content.Load<Texture2D>("3minutes");
            tenminutes = Content.Load<Texture2D>("10minutes");
            stopwatch = Content.Load<Texture2D>("stopwatch");
            board.LoadContent(content);  //Loads all the piece sprites
            chessboardSprite = Content.Load<Texture2D>("Board"); //Loads the sprite for the chess board
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            if (screen1 == true) //Main menu
            {
                MouseState mouse = Mouse.GetState();
                //gets the current state of the mouse for example XY position and button states
                Rectangle mouse2 = new Rectangle(mouse.X, mouse.Y, 1, 1);
                //creates rectangle with the position of the mouse

                if (leftclickPressed == false && mouse2.Intersects(playbutton) && mouse.LeftButton == ButtonState.Pressed)
                {
                    leftclickPressed = true;
                    //If you have not pressed left click in the previous frame and your mouse is over the sprite and have pressed it in the current frame pressed is true
                }

                if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released)
                {
                    // if you have pressed left click in the previous frame and have now released left click, there will be an output
                    leftclickPressed = false;

                    screen1 = false;
                    screen2 = true;
                }
            }
            if (screen2 == true) //Time selection menu
            {
                foreach (Rectangle button in minuteButtons) 
                {
                    MouseState mouse = Mouse.GetState();
                    Rectangle mouse2 = new Rectangle(mouse.X, mouse.Y, 1, 1);
                    if (mouse2.Intersects(button)) //Checks if the user is clicking the minute buttons
                    {
                        if (leftclickPressed == false && mouse.LeftButton == ButtonState.Pressed)
                        {
                            leftclickPressed = true;
                        }
                        if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released)
                        {
                            leftclickPressed = false;
                            if (button == minuteButtons[0]) //Sets time to 1 minute
                            {
                                board.duration = 60;
                                board.timerW = "01:00";
                                board.timerB = "01:00";
                                screen2 = false;
                                startGame = true;
                                //changes screen
                            }
                            else if (button == minuteButtons[1]) //Sets time to 3 minutes
                            {
                                board.duration = 180;
                                board.timerW = "03:00";
                                board.timerB = "03:00";
                                screen2 = false;
                                startGame = true;
                            }
                            else if (button == minuteButtons[2]) //Sets time to 10 minutes
                            {
                                board.duration = 600;
                                board.timerW = "10:00";
                                board.timerB = "10:00";
                                screen2 = false;
                                startGame = true;
                            }
                        }
                    }
                }
            }
            if (startGame == true)
            {
                board.Update(gameTime); //updates board

                MouseState mouse = Mouse.GetState();
                //gets the current state of the mouse for example XY position and button states
                Rectangle mouse2 = new Rectangle(mouse.X, mouse.Y, 1, 1);
                //creates rectangle with the position of the mouse
                if (mouse2.Intersects(endgamebutton))
                {
                    if (leftclickPressed == false && mouse2.Intersects(endgamebutton) && mouse.LeftButton == ButtonState.Pressed)
                    {
                        leftclickPressed = true;
                        //If you have not pressed left click in the previous frame and your mouse is over the sprite and have pressed it in the current frame pressed is true
                    }

                    if (leftclickPressed == true && mouse.LeftButton == ButtonState.Released)
                    {
                        // if you have pressed left click in the previous frame and have now released left click, there will be an output
                        leftclickPressed = false;

                        startGame = false;
                        screen1 = true;
                    }
                }
                    

            }
                
            if (board.reload == true)
            {
                board.LoadContent(Content);
                board.reload = false;
            }
            //Updates each chess piece in the chessboard array
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            
            _spriteBatch.Begin();
            if (screen1 == true) //Main menu
            {
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.DrawString(Title, "Chess", new Vector2(310, 75), Color.White);
                _spriteBatch.Draw(playButton, playbutton, Color.White);
                board.SetupBoard();
            }
            if (screen2 == true) //Time selection screen
            {
                GraphicsDevice.Clear(Color.DimGray);
                _spriteBatch.DrawString(Title, "Time", new Vector2(190, 75), Color.White);
                _spriteBatch.Draw(stopwatch, new Rectangle(185,150,160,200), Color.White);    
                _spriteBatch.Draw(oneminute, minuteButtons[0], Color.White);
                _spriteBatch.Draw(threeminutes, minuteButtons[1], Color.White);
                _spriteBatch.Draw(tenminutes, minuteButtons[2], Color.White);
            }
            if (startGame == true) //Chessboard
            {
                GraphicsDevice.Clear(Color.DimGray);
                _spriteBatch.Draw(chessboardSprite, new Rectangle(160, 0, 480, 480), Color.White);
                _spriteBatch.Draw(endgameButton, endgamebutton,Color.White);
                //Draws the chess board sprite
                board.Draw(_spriteBatch);
            }
         
            //Loads all the piece sprites
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
