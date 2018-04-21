﻿using Hugh.Concrete;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using TiledSharp;

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

        Texture2D BorderTexture;

        string LevelName;
        
        public HughGame(string levelName)
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            LevelName = levelName;
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

            Multiverse = new Multiverse(this, map, 2);

            Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.ApplyChanges();

            ViewportMain = GraphicsDevice.Viewport;

            Viewport viewportTop, viewportBottom;

            viewportTop = ViewportMain;
            viewportTop.Height = viewportTop.Height / 2;

            viewportBottom = ViewportMain;
            viewportBottom.Height = viewportBottom.Height / 2;
            viewportBottom.Y = viewportBottom.Height;

            Multiverse.GetWorld(1).Viewport = viewportTop;
            Multiverse.GetWorld(2).Viewport = viewportBottom;

            BorderTexture = new Texture2D(GraphicsDevice, 1, 1);
            BorderTexture.SetData(new[] { Color.Black });
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

            GraphicsDevice.Viewport = ViewportMain;

            SpriteBatch.Begin();
            // Border between the universes
            SpriteBatch.Draw(BorderTexture, new Rectangle(0, ViewportMain.Height / 2 - 1, ViewportMain.Width, 2),
                             Color.Black);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}