using Microsoft.Xna.Framework;
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

        private string type;

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect {
            get { return new Rectangle(SIZE * column, SIZE * row, SIZE, SIZE); }
        }

        public float X {get { return (float)this.x * SIZE; }}
        public float Y {get { return (float)this.y * SIZE; }}

        public Tile(int row, int column, int x, int y, string type)
        {
            this.row = row;
            this.column = column;
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public bool IsGround()
        {
            return "ground".Equals(this.type);
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

        public bool IsOnFloor { get; set; }

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
        private Texture2D tilesetTexture;

        // The interactive tile layer (the only layer for now)
        private Player player;
        private Tile[] tiles;
        private int width;
        private int height;

        private Game1 game;

        public Viewport Viewport { get; set; }

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

            var tileset = map.Tilesets[0];
            this.tilesetTexture = game.Content.Load<Texture2D>(tileset.Name.ToString());

            AddTilesFromLayer(findLayer(map, "universal"), tileset);
            AddTilesFromLayer(findLayer(map, string.Format("world{0}", worldId)), tileset);
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

        private void AddTilesFromLayer(TmxLayer layer, TmxTileset tileset)
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

                int tilesetTilesWidth = this.tilesetTexture.Width / Tile.SIZE;

                int tileFrame = gid - 1;
                int column = tileFrame % tilesetTilesWidth;
                int row = tileFrame / tilesetTilesWidth;

                int x = i % this.width;
                int y = i / this.width;

                string tileType = GetTilesetTileType(tileset, tileFrame);

                if ("player".Equals(tileType))
                {
                    player = new Player(row, column, new Vector2(x * Tile.SIZE, y * Tile.SIZE));
                    continue;
                } else {
                    this.tiles[y * this.width + x] = new Tile(row, column, x, y, tileType);
                }
            }
        }

        private string GetTilesetTileType(TmxTileset tileset, int id)
        {
            TmxTilesetTile tilesetTile = TileForId(tileset, id);
            
            if (tilesetTile == null || ! tilesetTile.Properties.ContainsKey("type")) {
                return null;
            }

            return tilesetTile.Properties["type"];
        }

        private TmxTilesetTile TileForId(TmxTileset tileset, int id)
        {
            // Terribly inefficient.. we could pretty easily optimize this if loading is too slow
            foreach (TmxTilesetTile t in tileset.Tiles) {
                if (t.Id == id) {
                    return t;
                }
            }
            return null;
        }

        public void Draw()
        {
            game.GraphicsDevice.Viewport = this.Viewport;

            game.spriteBatch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   null, null, null, null,
                                   GetCameraTransform());

            DrawTiles();

            int playerX = (int)player.position.X;
            int playerY = (int)player.position.Y;
            var playerPositionRect = new Rectangle(playerX, playerY, Tile.SIZE, Tile.SIZE);
            game.spriteBatch.Draw(tilesetTexture, playerPositionRect, player.TilesetRect, Color.White);

            game.spriteBatch.End();
        }

        private Matrix GetCameraTransform()
        {
            int worldWidth = this.width * Tile.SIZE;
            int worldHeight = this.height * Tile.SIZE;

            int viewportWidth = this.Viewport.Width;
            int viewportHeight = this.Viewport.Height;

            int cameraX;
            int cameraY;

            if (worldWidth <= viewportWidth) {
                cameraX = (worldWidth - viewportWidth) / 2;
            } else {
                int playerCenterX = (int)player.position.X + (int)Tile.SIZE / 2;
                cameraX = playerCenterX - viewportWidth / 2;
                cameraX = Math.Max(0, Math.Min(worldWidth - viewportWidth, cameraX));
            }

            if (worldHeight <= viewportHeight) {
                cameraY = (worldHeight - viewportHeight) / 2;
            } else {
                int playerCenterY = (int)player.position.Y + (int)Tile.SIZE / 2;
                cameraY = playerCenterY - viewportHeight / 2;
                cameraY = Math.Max(0, Math.Min(worldHeight - viewportHeight, cameraY));
            }

            return Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-cameraX, -cameraY, 0));
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
                game.spriteBatch.Draw(tilesetTexture, positionRect, t.TilesetRect, Color.White);
            }
        }

        public void Update(float dt)
        {
            // TODO Refactor out the player controls and logic into the Player class

            if (Controller.isLeftPressed() && !Controller.isRightPressed())
            {
                player.velocity.X -= 3 * dt;
            } else if (Controller.isRightPressed() && !Controller.isLeftPressed()) {
                player.velocity.X += 3 * dt;
            } else {
                const float FRICTION = 8f;

                if (player.velocity.X > 0) {
                    player.velocity.X = Math.Max(0, player.velocity.X - FRICTION * dt);
                } else if (player.velocity.X < 0) {
                    player.velocity.X = Math.Min(0, player.velocity.X + FRICTION * dt);
                }
            }

            const float GRAVITY = 9.8f; 

            if (Controller.isUpPressed() && player.IsOnFloor)
            {
                player.velocity.Y = - 6f;
            }

            // Gravity.
            player.velocity.Y += GRAVITY * dt;

            // May be set in HandleFloorCollisions
            player.IsOnFloor = false;

            while (HandleFloorCollisions())
            {
            }

            player.position.X += player.velocity.X;
            player.position.Y += player.velocity.Y;
        }

        private bool HandleFloorCollisions()
        {
            // Note: If collisions are handled properly, initialRect should never overlap
            Rectangle initialRect = ComputeEntityRect(player.position);
            Rectangle finalRect   = ComputeEntityRect(player.position + player.velocity);

            Rectangle aabb = ComputeAabb(initialRect, finalRect);

            List<Tile> intersectingTiles = GetTilesWithinRect(aabb);

            // TODO: instead of ignoring non-ground tiles, handle them appropriately!
            intersectingTiles = intersectingTiles.FindAll((tile) => tile.IsGround());

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
                    player.IsOnFloor = true;
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
            // Comparing floats caused issues with corners
            return (int)Math.Abs(player.position.Y - t.Y) >= (int)Math.Abs(player.position.X - t.X);
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

        Viewport viewportMain;
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

            // TODO draw a border between the world viewports

            base.Draw(gameTime);
        }
    }
}
