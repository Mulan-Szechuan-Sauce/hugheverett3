using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using TiledSharp;

namespace Hugh {
    class Controller {
        private static bool IsKeyDown(Keys key) {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static bool isLeftPressed() {
            return IsKeyDown(Keys.Left) || IsKeyDown(Keys.A);
        }

        public static bool isRightPressed() {
            return IsKeyDown(Keys.Right) || IsKeyDown(Keys.E);
        }

        public static bool isUpPressed() {
            return IsKeyDown(Keys.Up) || IsKeyDown(Keys.OemComma);
        }

        public static bool isDownPressed() {
            return IsKeyDown(Keys.Down) || IsKeyDown(Keys.O);
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;

        public SpriteBatch spriteBatch;

        TmxMap map;
        Texture2D tileset;

        int tileWidth;
        int tileHeight;
        int tilesetTilesWide;
        int tilesetTilesHigh;
        
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = new TmxMap("Content/level1.tmx");

            tileset = Content.Load<Texture2D>(map.Tilesets[0].Name.ToString());

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;

            graphics.PreferredBackBufferHeight = map.Height * tileHeight * 2;
            graphics.PreferredBackBufferWidth = map.Width * tileWidth;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            RenderLayer(map.Layers[0], Vector2.Zero);
            RenderLayer(map.Layers[1], Vector2.Zero);

            int mapHeight = map.TileHeight * map.Height;

            RenderLayer(map.Layers[0], new Vector2(0, mapHeight));
            RenderLayer(map.Layers[2], new Vector2(0, mapHeight));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void RenderLayer(TmxLayer layer, Vector2 offset)
        {
            for (var i = 0; i < layer.Tiles.Count; i++)
            {
                int gid = layer.Tiles[i].Gid;

                // Empty tile, do nothing
                if (gid == 0)
                {
                    continue;
                }

                int tileFrame = gid - 1;
                int column = tileFrame % tilesetTilesWide;
                int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                float x = (i % map.Width) * map.TileWidth + offset.X;
                float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight + offset.Y;

                var tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                var positionRec = new Rectangle((int)x, (int)y, tileWidth, tileHeight);

                spriteBatch.Draw(tileset, positionRec, tilesetRec, Color.White);
            }
        }
    }
}
