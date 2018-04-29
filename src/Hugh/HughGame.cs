using Hugh.Concrete;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using TiledSharp;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Hugh
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class HughGame : Game
    {
        GraphicsDeviceManager Graphics;

        public SpriteBatch SpriteBatch;

        Viewport ViewportMain;

        Multiverse Multiverse;

        private Desktop LevelSelectionDesktop;
        // TODO: Move the states to an enum
        private string State;
        private string InitialLevelName;

        private string LevelName;

        public SpriteFont GameFont;

        public TilesetManager TilesetManager;

        private List<string> LevelNames;
        private int CurrentLevelIndex;
        
        public HughGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            SetState("levelselection");
            InitialLevelName = null;
        }

        public HughGame(string levelName) : this()
        {
            InitialLevelName = levelName;
        }

        private void SetState(string state)
        {
            IsMouseVisible = "levelselection".Equals(state);
            State = state;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            TilesetManager = new TilesetManager(this);

            Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.ApplyChanges();

            ViewportMain = GraphicsDevice.Viewport;

            GameFont = Content.Load<SpriteFont>("GameFont");

            LoadLevelSelectionDesktop();

            if (InitialLevelName != null)
            {
                LoadLevel(InitialLevelName);
                State = "playing";
            }
        }

        private void LoadLevelSelectionDesktop()
        {
            Myra.MyraEnvironment.Game = this;

            // Add it to the desktop
            LevelSelectionDesktop = new Desktop();

            // TODO: _possibly_ move this into a config file
            LevelNames = new List<string>() {
                "intro_controls",
                "intro2",
                "intro3",
                "intro_momentum",
                "level2",
                "level3",
                "level4",
                "level5",
                "level6",
                "level8",
            };

            int index = 0;

            LevelNames.ForEach(levelName => {
                const int GRID_WIDTH = 5;
                int GRID_HEIGHT = (int)Math.Ceiling((float)LevelNames.Count / GRID_WIDTH);
                const int BUTTON_SIZE = 32;
                const int BUTTON_MARGIN = 8;
                const int BUTTON_REGION = BUTTON_SIZE + BUTTON_MARGIN;

                // Button
                var button = new Button {
                    Text = "" + (index + 1),
                    WidthHint = BUTTON_SIZE,
                    HeightHint = BUTTON_SIZE,
                    ContentHorizontalAlignment = HorizontalAlignment.Center,
                    ContentVerticalAlignment = VerticalAlignment.Center
                };

                int xPos = index % GRID_WIDTH;
                int yPos = index / GRID_WIDTH;

                button.XHint = ViewportMain.Width / 2 + BUTTON_REGION * (xPos - GRID_WIDTH / 2);
                button.YHint = ViewportMain.Height / 2 + BUTTON_REGION * (yPos - GRID_HEIGHT);

                var levelIndex = index;
                button.Up += (ob, ev) => LoadLevel(levelName, levelIndex);

                LevelSelectionDesktop.Widgets.Add(button);

                index++;
            });
        }

        private void LoadLevel(string levelName, int levelIndex = -1)
        {
            LevelName = levelName;
            CurrentLevelIndex = levelIndex;
            Console.WriteLine("Level index: " + CurrentLevelIndex);
            TmxMap map = new TmxMap("Content/" + levelName + ".tmx");
            TilesetManager.LoadMap(map);
            Multiverse = new Multiverse(this, map);
            Multiverse.SetViewport(ViewportMain);
            SetState("playing");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();

            if (State == "playing")
            {
                // Restart
                if (keyState.IsKeyDown(Keys.R))
                    LoadLevel(LevelName, CurrentLevelIndex);

                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Multiverse.Update(dt);

                if (Multiverse.HasWon && keyState.IsKeyDown(Keys.N) && HasNextLevel())
                {
                    LoadLevel(LevelNames[CurrentLevelIndex + 1], CurrentLevelIndex + 1);
                }
            }

            if (keyState.IsKeyDown(Keys.Escape))
                if (InitialLevelName == null)
                    SetState("levelselection");
                else
                    Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            if (State == "playing")
            {
                Multiverse.Draw();

                if (Multiverse.HasWon)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.DrawString(GameFont, "VERY NICE. GREAT SUCCESS!",
                                           new Vector2(10, 10), Color.Black);
                    if (HasNextLevel())
                        SpriteBatch.DrawString(GameFont, "Press N to continue...",
                                               new Vector2(10, 30), Color.Black);
                    SpriteBatch.End();
                }
            }
            else if (State == "levelselection")
            {
                var presParams = GraphicsDevice.PresentationParameters;

                LevelSelectionDesktop.Bounds = new Rectangle(0, 0, presParams.BackBufferWidth,
                                                             presParams.BackBufferHeight);
                LevelSelectionDesktop.Render();
            }

            base.Draw(gameTime);
        }

        private bool HasNextLevel()
        {
            return CurrentLevelIndex >= 0 && CurrentLevelIndex < LevelNames.Count - 1;
        }
    }
}
