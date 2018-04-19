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

    class Tile {
        // By design, all tiles are 32x32 px
        public const int SIZE = 32;

        private int row;
        private int column;

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect {
            get { return new Rectangle(SIZE * column, SIZE * row, SIZE, SIZE); }
        }

        public Tile(int row, int column) {
            this.row = row;
            this.column = column;
        }
    }

    class World
    {
        /*
         * Note:
         * This should stay be a per-world property, since we can
         * potentiallly have different tileset for each world.
         */
        private Texture2D tileset;

        // The interactive tile layer (the only layer for now)
        private Tile[] tiles;
        private int width;
        private int height;

        private Game1 game;

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public World(Game1 game, TmxMap map, int worldId)
        {
            this.game = game;
            this.width = map.Width;
            this.height = map.Height;
            this.tiles = new Tile[this.width * this.height];
            this.tileset = game.Content.Load<Texture2D>(map.Tilesets[0].Name.ToString());

            // TODO select the layer properly via the worldId

            // Interactive layer
            addTilesFromLayer(map.Layers[0]);
            // World spceific layer
            addTilesFromLayer(map.Layers[worldId]);
        }

        private void addTilesFromLayer(TmxLayer layer)
        {
            for (var i = 0; i < layer.Tiles.Count; i++)
            {
                int gid = layer.Tiles[i].Gid;

                // Empty tile, do nothing
                if (gid == 0)
                {
                    continue;
                }

                int tilesetTilesWidth = this.tileset.Width / Tile.SIZE;

                int tileFrame = gid - 1;
                int column = tileFrame % tilesetTilesWidth;
                int row = tileFrame / tilesetTilesWidth;

                int x = i % this.width;
                int y = i / this.width;

                this.tiles[y * this.width + x] = new Tile(row, column);
            }
        }

        public void Draw()
        {
            game.spriteBatch.Begin();

            for (int i = 0; i < this.width * this.height; i++)
            {
                Tile tile = this.tiles[i];

                if (tile == null) {
                    continue;
                }

                int xIndex = i % this.width;
                int yIndex = i / this.width;

                int x = xIndex * Tile.SIZE;
                int y = yIndex * Tile.SIZE;

                var positionRec = new Rectangle((int)x, (int)y, Tile.SIZE, Tile.SIZE);
                game.spriteBatch.Draw(tileset, positionRec, tile.TilesetRect, Color.White);
            }

            game.spriteBatch.End();
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;

        public SpriteBatch spriteBatch;

        World world1, world2;
        
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

            TmxMap map = new TmxMap("Content/level1.tmx");

            world1 = new World(this, map, 1);
            world2 = new World(this, map, 2);

            graphics.PreferredBackBufferHeight = world1.Height * Tile.SIZE * 2;
            graphics.PreferredBackBufferWidth = world1.Width * Tile.SIZE;
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

            // TODO: Draw them in different ViewPorts (in different parts of the screen)
            world1.Draw();
            //world2.Draw();

            base.Draw(gameTime);
        }
    }
}
