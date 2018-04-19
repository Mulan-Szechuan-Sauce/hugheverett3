﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using TiledSharp;

namespace Hugh {
    class Controller
    {
        private static bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static bool isLeftPressed()
        {
            return IsKeyDown(Keys.Left) || IsKeyDown(Keys.A);
        }

        public static bool isRightPressed()
        {
            return IsKeyDown(Keys.Right) || IsKeyDown(Keys.E);
        }

        public static bool isUpPressed()
        {
            return IsKeyDown(Keys.Up) || IsKeyDown(Keys.OemComma);
        }

        public static bool isDownPressed()
        {
            return IsKeyDown(Keys.Down) || IsKeyDown(Keys.O);
        }
    }

    class Tile
    {
        // By design, all tiles are 32x32 px
        public const int SIZE = 32;

        private int row;
        private int column;

        private int x;
        private int y;

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect {
            get { return new Rectangle(SIZE * column, SIZE * row, SIZE, SIZE); }
        }

        public float X {get { return (float)this.x * SIZE; }}
        public float Y {get { return (float)this.y * SIZE; }}

        public Tile(int row, int column, int x, int y)
        {
            this.row = row;
            this.column = column;
            this.x = x;
            this.y = y;
        }
    }

    /*
     * TODO: In the future (post-prototype), dynamic objects should be in
     * their own tileset, and be allowed to differ in size.
     */
    class Player {
        // By design, the player is 32x32 px (for now?)
        private const int SIZE = 32;

        public Vector2 position;
        public Vector2 velocity;
        private int row;
        private int column;

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect {
            get { return new Rectangle(SIZE * column, SIZE * row, SIZE, SIZE); }
        }

        public Player(int row, int column, Vector2 position)
        {
            this.row = row;
            this.column = column;
            this.position = position;
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
        private Player player;
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

            addTilesFromLayer(findLayer(map, "universal"));
            addTilesFromLayer(findLayer(map, string.Format("world{0}", worldId)));
        }

        private TmxLayer findLayer(TmxMap map, string name)
        {
            foreach (TmxLayer layer in map.Layers)
            {
                if (name.Equals(layer.Name))
                {
                    return layer;
                }
            }
            // Suck it up buttercup
            return null;
        }

        private void addTilesFromLayer(TmxLayer layer)
        {
            for (var i = 0; i < layer.Tiles.Count; i++)
            {
                TmxLayerTile tile = layer.Tiles[i];
                int gid = tile.Gid;

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

                // FIXME this should check a Tiled property (I think the "Type" property)
                if (tileFrame == 5)
                {
                    player = new Player(row, column, new Vector2(x * Tile.SIZE, y * Tile.SIZE));
                    continue;
                }

                this.tiles[y * this.width + x] = new Tile(row, column, x, y);
            }
        }

        public void Draw()
        {
            game.spriteBatch.Begin();

            DrawTiles();

            int playerX = (int)player.position.X;
            int playerY = (int)player.position.Y;
            var playerPositionRect = new Rectangle(playerX, playerY, Tile.SIZE, Tile.SIZE);
            game.spriteBatch.Draw(tileset, playerPositionRect, player.TilesetRect, Color.White);

            game.spriteBatch.End();
        }

        private void DrawTiles()
        {
            for (int i = 0; i < this.width * this.height; i++)
            {
                Tile t = this.tiles[i];

                if (t == null)
                {
                    continue;
                }

                var positionRect = new Rectangle((int)t.X, (int)t.Y, Tile.SIZE, Tile.SIZE);
                game.spriteBatch.Draw(tileset, positionRect, t.TilesetRect, Color.White);
            }
        }

        public void Update(float dt)
        {
            // TODO Refactor out the player controls and logic into the Player class

            if (Controller.isLeftPressed())
            {
                player.velocity.X -= 3 * dt;
            }

            if (Controller.isRightPressed())
            {
                player.velocity.X += 3 * dt;
            }

            const float GRAVITY = 9.8f; 

            // Weird jumping functionality for testing - It should only jump if grounded
            if (Controller.isUpPressed())
            {
                player.velocity.Y -= GRAVITY * dt;
                player.velocity.Y -= 5 * dt;
            }

            // Gravity.
            player.velocity.Y += GRAVITY * dt;

            while (HandleCollisions())
            {
            }

            player.position.X += player.velocity.X;
            player.position.Y += player.velocity.Y;
        }

        private bool HandleCollisions()
        {
            // Note: If collisions are handled properly, initialRect should never overlap
            Rectangle initialRect = ComputeEntityRect(player.position);
            Rectangle finalRect   = ComputeEntityRect(player.position + player.velocity);

            Rectangle aabb = ComputeAabb(initialRect, finalRect);

            List<Tile> intersectingTiles = GetTilesWithinRect(aabb);
            if (intersectingTiles.Count == 0)
            {
                return false;
            }

            // TODO: Just get the minimum manhattan distance, don't bother sorting
            intersectingTiles.Sort((a, b) => {
                    // Sort by manhattan distance (faster than euclidean, and "good enough")
                    float distA = Math.Abs(a.X - player.position.X) + Math.Abs(a.Y - player.position.Y);
                    float distB = Math.Abs(b.X - player.position.X) + Math.Abs(b.Y - player.position.Y);

                    return (distA > distB) ? 1 : -1;
                });

            Tile t = intersectingTiles[0];

            if (IsVerticalCollision(t))
            {
                if (initialRect.Y < t.Y)
                {
                    // Floor hit
                    player.position.Y = (float)Math.Floor(t.Y - Tile.SIZE);
                    player.velocity.Y = 0;
                } else {
                    // Ceiling hit
                    player.position.Y = (float)Math.Ceiling(t.Y + Tile.SIZE);
                    player.velocity.Y = 0;
                }
            } else {
                if (initialRect.X < t.X)
                {
                    // Right hit
                    player.position.X = (float)Math.Floor(t.X - Tile.SIZE);
                    player.velocity.X = 0;
                } else {
                    // Left hit
                    player.position.X = (float)Math.Ceiling(t.X + Tile.SIZE);
                    player.velocity.X = 0;
                }
            }

            return true;
        }

        // TODO: make this work for an arbitrary dynamic object
        private bool IsVerticalCollision(Tile t)
        {
            return Math.Abs(player.position.Y - t.Y) > Math.Abs(player.position.X - t.X);
        }

        private List<Tile> GetTilesWithinRect(Rectangle r)
        {
            List<Tile> tiles = new List<Tile>();
            
            int x1 = (int)Math.Floor((float)r.X / Tile.SIZE);
            int x2 = (int)Math.Ceiling((float)(r.X + r.Width) / Tile.SIZE);
            int y1 = (int)Math.Floor((float)r.Y / Tile.SIZE);
            int y2 = (int)Math.Ceiling((float)(r.Y + r.Height) / Tile.SIZE);
            
            for (int x = x1; x < x2; x++)
            {
                for (int y = y1; y < y2; y++)
                {
                    if (x >= this.width || x < 0 || y >= this.height || y < 0)
                    {
                        continue;
                    }

                    Tile tile = this.tiles[y * this.width + x];
                    if (tile != null)
                    {
                        tiles.Add(tile);
                    }
                }
            }
            
            return tiles;
        }

        private static Rectangle ComputeAabb(Rectangle a, Rectangle b)
        {
            int x = Math.Min(a.X, b.X);
            int y = Math.Min(a.Y, b.Y);
            int width = Math.Max(a.X + a.Width, b.X + b.Width) - x;
            int height = Math.Max(a.Y + a.Height, b.Y + b.Height) - y;

            return new Rectangle(x, y, width, height);
        }

        private static Rectangle ComputeEntityRect(Vector2 position)
        {
            return new Rectangle((int)position.X, (int)position.Y, Tile.SIZE, Tile.SIZE);
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;

        public SpriteBatch spriteBatch;

        Viewport viewportTop, viewportBottom, viewportMain;
        World world1, world2;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            TmxMap map = new TmxMap("Content/level1.tmx");

            world1 = new World(this, map, 1);
            world2 = new World(this, map, 2);

            graphics.PreferredBackBufferHeight = world1.Height * Tile.SIZE * 2;
            graphics.PreferredBackBufferWidth = world1.Width * Tile.SIZE;
            graphics.ApplyChanges();

            viewportMain = GraphicsDevice.Viewport;

            viewportTop = viewportMain;
            viewportTop.Height = viewportTop.Height / 2;

            viewportBottom = viewportMain;
            viewportBottom.Height = viewportBottom.Height / 2;
            viewportBottom.Y = viewportBottom.Height;
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

            GraphicsDevice.Viewport = viewportTop;
            world1.Draw();
            GraphicsDevice.Viewport = viewportBottom;
            world2.Draw();
            GraphicsDevice.Viewport = viewportMain;

            base.Draw(gameTime);
        }
    }
}
