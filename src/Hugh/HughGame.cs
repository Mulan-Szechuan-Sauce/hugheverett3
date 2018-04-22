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

        string LevelName;

        private Desktop _host;
        
        public HughGame(string levelName)
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            LevelName = levelName;

            IsMouseVisible = true;
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

            TmxMap map = new TmxMap("Content/" + LevelName + ".tmx");

            Multiverse = new Multiverse(this, map);

            Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.ApplyChanges();

            ViewportMain = GraphicsDevice.Viewport;
            Multiverse.SetViewport(ViewportMain);
            Myra.MyraEnvironment.Game = this;

            // Add it to the desktop
            _host = new Desktop();

            for (int i = 0; i < 4; i++)
            {
                const int BUTTON_SIZE = 24;
                const int BUTTON_MARGIN = 8;
                
                // Button
                var button = new Button {
                    Text = "" + (i + 1),
                    WidthHint = BUTTON_SIZE,
                    HeightHint = BUTTON_SIZE,
                    ContentHorizontalAlignment = HorizontalAlignment.Center
                };

                button.XHint = (BUTTON_SIZE + BUTTON_MARGIN) * i;

                button.Up += (ob, ev) => {
                    Button evButton = (Button)ob;
                    string text = evButton.Text;
                    LevelName = "level" + text;
                    LoadContent();
                };

                _host.Widgets.Add(button);
            }
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Restart
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                LoadContent();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Multiverse.Update(dt);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            Multiverse.Draw();

            _host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
_host.Render();

            base.Draw(gameTime);
        }
    }
}
