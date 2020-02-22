using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;


namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Sprites
        Sprite[,] gameBoard;

        //Mouse
        MouseState mouseCurrent;
        MouseState mousePrevious;

        //Ints
        int gameNumber = 0;
        int numberOfBombs;
        int gameBoardSize;
        int numberOfBombsDiscovered;
        bool gameOver;
        bool winner;

        int seconds = 0;
        int mintues = 0;

        byte[] randomBombs;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameBoardSize = 20;
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 25 * gameBoardSize;
            graphics.PreferredBackBufferHeight = 25 * gameBoardSize + 25;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            gameBoard = new Sprite[gameBoardSize, gameBoardSize];
            randomBombs = new byte[gameBoardSize * gameBoardSize];
        }

        protected override void Initialize()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameNumber == 0)
                        gameBoard[i, j] = new Sprite(new Vector2(i * 25, j * 25 + 25), null, new Rectangle(0, 0, 25, 25));
                    else
                    {
                        gameBoard[i, j].clearFeild();
                        gameBoard[i, j].GetSetSpriteTexture2D = null;
                    }
                    randomBombs[i * gameBoardSize + j] = (byte)(i * gameBoardSize + j);
                }
            }
            numberOfBombs = 50;
            locateBombs();
            gameOver = false;
            winner = false;
            numberOfBombsDiscovered = numberOfBombs;
            gameNumber++;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    gameBoard[i, j].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("button");
                }
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();
            seconds = (int)gameTime.TotalGameTime.TotalSeconds % 60;

            if (mousePrevious.LeftButton == ButtonState.Pressed && mouseCurrent.LeftButton == ButtonState.Released)
            {
                if (mouseCurrent.Y / 25 - 1 < 0 || mouseCurrent.X / 25 < 0) return;

                if ((gameOver || winner) && mouseCurrent.X < 300 && mouseCurrent.X > 200 && mouseCurrent.Y > 250 && mouseCurrent.Y < 350)
                {
                    reloadGame(gameTime);
                    return;
                }
                if (gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].Bombed())
                {
                    showAllBombs();
                    gameOver = true;
                }
                else
                {
                    if (gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].GetSetSpriteTexture2D == this.Content.Load<Texture2D>("buttonFlagged")) return;
                    gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                    freeLand(mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1);
                }
            }
            if (mousePrevious.RightButton == ButtonState.Pressed && mouseCurrent.RightButton == ButtonState.Released)
            {
                if (gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].GetSetSpriteTexture2D == this.Content.Load<Texture2D>("buttonFlagged"))
                {
                    gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("button");
                    numberOfBombsDiscovered++;
                }
                else
                {
                    gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonFlagged");
                    if (gameBoard[mouseCurrent.X / 25, mouseCurrent.Y / 25 - 1].Bombed()) numberOfBombs--;
                    numberOfBombsDiscovered--;
                    if (numberOfBombs == 0)
                        winner = true;
                }
            }

            base.Update(gameTime);
        }

        public void reloadGame(GameTime gameTime)
        {
            Initialize();
            LoadContent();
            Draw(gameTime);
            Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            SpriteFont spriteFont = this.Content.Load<SpriteFont>("myFont");
            GraphicsDevice.Clear(Color.LightGray);
            spriteBatch.Begin();
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    spriteBatch.Draw(gameBoard[i, j].GetSetSpriteTexture2D, gameBoard[i, j].GetSetSpriteVector2D, gameBoard[i, j].GetSetSpriteRectangle, Color.White);
                    if (gameBoard[i, j].GetSetSpriteTexture2D == this.Content.Load<Texture2D>("buttonPressed") && gameBoard[i, j].getNumberNearMe() != 0)
                    {
                        spriteBatch.DrawString(spriteFont, gameBoard[i, j].getNumberNearMe().ToString(), new Vector2(gameBoard[i, j].GetSetSpriteVector2D.X + 7, gameBoard[i, j].GetSetSpriteVector2D.Y + 3), Color.Blue);
                    }
                }
            }
            spriteBatch.DrawString(spriteFont, numberOfBombsDiscovered.ToString(), new Vector2(10, 0), Color.Black);
            spriteBatch.DrawString(spriteFont, mintues.ToString() + " : " + seconds.ToString(), new Vector2(430, 0), Color.Black);
            if (gameOver)
            {
                spriteBatch.Draw(this.Content.Load<Texture2D>("gameOver"), new Vector2(50, 170), new Rectangle(0, 0, 419, 82), Color.White);
                spriteBatch.Draw(this.Content.Load<Texture2D>("reloadButton"), new Vector2(200, 250), new Rectangle(0, 0, 200, 200), Color.White);
            } else if (numberOfBombs == 0 && winner)
            {
                spriteBatch.Draw(this.Content.Load<Texture2D>("winner"), new Vector2(95, 170), new Rectangle(0, 0, 310, 98), Color.White);
                spriteBatch.Draw(this.Content.Load<Texture2D>("reloadButton"), new Vector2(200, 250), new Rectangle(0, 0, 200, 200), Color.White);
            }
            base.Draw(gameTime);
            spriteBatch.End();

        }

        private void locateBombs()
        {
            System.Random random = new System.Random();            
            int randomNumber1, randomNumber2;
            for (int i = 0; i < numberOfBombs; i++)
            {
                randomNumber1 = random.Next(0, 50);
                randomNumber2 = random.Next(0, 50);
                gameBoard[randomNumber1, randomNumber2].placeBomb();

            }
            numberOfBombs = 0;

            placeNumbrers();
        }

        private void placeNumbrers()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j].Bombed())
                    {
                        numberOfBombs++;
                        // Determaning the number on the box 
                        if (i > 0) gameBoard[i - 1, j].incNumberNearMe();
                        if (j > 0) gameBoard[i, j - 1].incNumberNearMe();
                        if (i < gameBoardSize) gameBoard[i + 1, j].incNumberNearMe();
                        if (j < gameBoardSize) gameBoard[i, j + 1].incNumberNearMe();
                        if (i > 0 && j < gameBoardSize) gameBoard[i - 1, j + 1].incNumberNearMe();
                        if (i < gameBoardSize && j > 0) gameBoard[i + 1, j - 1].incNumberNearMe();
                        if (i > 0 && j > 0) gameBoard[i - 1, j - 1].incNumberNearMe();
                        if (j < gameBoardSize && i < gameBoardSize) gameBoard[i + 1, j + 1].incNumberNearMe();
                    }
                }
            }
        }
        private void showAllBombs()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j].Bombed())
                    {
                        gameBoard[i, j].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonBombed");
                    }
                }
            }
        }
        private void freeLand(int x, int y)
        {
            if (gameBoard[x, y].getNumberNearMe() != 0)
                return;
            if (x > 0 && !gameBoard[x - 1, y].Bombed() && gameBoard[x - 1, y].getNumberNearMe() >= 0 && !gameBoard[x - 1, y].isDicovered)
            {
                gameBoard[x - 1, y].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x - 1, y].isDicovered = true;
                freeLand(x - 1, y);
            }
            if (x < gameBoardSize - 1 && !gameBoard[x + 1, y].Bombed() && gameBoard[x + 1, y].getNumberNearMe() >= 0 && !gameBoard[x + 1, y].isDicovered)
            {
                gameBoard[x + 1, y].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x + 1, y].isDicovered = true;
                freeLand(x + 1, y);
            }
            if (y > 0 && !gameBoard[x, y - 1].Bombed() && gameBoard[x, y - 1].getNumberNearMe() >= 0 && !gameBoard[x, y - 1].isDicovered)
            {
                gameBoard[x, y - 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x, y - 1].isDicovered = true;
                freeLand(x, y - 1);
            }
            if (y < gameBoardSize - 1 && !gameBoard[x, y + 1].Bombed() && gameBoard[x, y + 1].getNumberNearMe() >= 0 && !gameBoard[x, y + 1].isDicovered)
            {
                gameBoard[x, y + 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x, y + 1].isDicovered = true;
                freeLand(x, y + 1);
            }
            if (x < gameBoardSize - 1 && y < gameBoardSize - 1 && !gameBoard[x + 1, y + 1].Bombed() && gameBoard[x + 1, y + 1].getNumberNearMe() >= 0 && !gameBoard[x + 1, y + 1].isDicovered)
            {
                gameBoard[x + 1, y + 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x + 1, y + 1].isDicovered = true;
                freeLand(x + 1, y + 1);
            }
            if (x > 0 && y > 0 && !gameBoard[x - 1, y - 1].Bombed() && gameBoard[x - 1, y - 1].getNumberNearMe() >= 0 && !gameBoard[x - 1, y - 1].isDicovered)
            {
                gameBoard[x - 1, y - 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x - 1, y - 1].isDicovered = true;
                freeLand(x - 1, y - 1);
            }

            if (x < gameBoardSize - 1 && y > 0 && !gameBoard[x + 1, y - 1].Bombed() && gameBoard[x + 1, y - 1].getNumberNearMe() >= 0 && !gameBoard[x + 1, y - 1].isDicovered)
            {
                gameBoard[x + 1, y - 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x + 1, y - 1].isDicovered = true;
                freeLand(x + 1, y - 1);
            }

            if (x > 0 && y < gameBoardSize - 1 && !gameBoard[x - 1, y + 1].Bombed() && gameBoard[x - 1, y + 1].getNumberNearMe() >= 0 && !gameBoard[x - 1, y + 1].isDicovered)
            {
                gameBoard[x - 1, y + 1].GetSetSpriteTexture2D = this.Content.Load<Texture2D>("buttonPressed");
                gameBoard[x - 1, y + 1].isDicovered = true;
                freeLand(x - 1, y + 1);
            }
        }
    }
}
