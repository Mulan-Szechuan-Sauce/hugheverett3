using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using TiledSharp;

namespace Hugh {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;

        public SpriteBatch spriteBatch;

        Viewport viewportMain;
        World world1, world2;

        Texture2D borderTexture;

        string levelName;
        
        public Game1(string levelName)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.levelName = levelName;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TmxMap map = new TmxMap("Content/" + this.levelName + ".tmx");

            world1 = new World(this, map, 1);
            world2 = new World(this, map, 2);

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();

            viewportMain = GraphicsDevice.Viewport;

            Viewport viewportTop, viewportBottom;

            viewportTop = viewportMain;
            viewportTop.Height = viewportTop.Height / 2;

            viewportBottom = viewportMain;
            viewportBottom.Height = viewportBottom.Height / 2;
            viewportBottom.Y = viewportBottom.Height;

            world1.Viewport = viewportTop;
            world2.Viewport = viewportBottom;

            borderTexture = new Texture2D(GraphicsDevice, 1, 1);
            borderTexture.SetData(new[] { Color.Black });
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
            world1.Update(dt);
            world2.Update(dt);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            world1.Draw();
            world2.Draw();

            GraphicsDevice.Viewport = viewportMain;

            spriteBatch.Begin();
            // Border between the universes
            spriteBatch.Draw(borderTexture, new Rectangle(0, viewportMain.Height / 2 - 1, viewportMain.Width, 2),
                             Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
