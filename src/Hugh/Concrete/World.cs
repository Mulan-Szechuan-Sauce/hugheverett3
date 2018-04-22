using Hugh.Concrete;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using TiledSharp;

namespace Hugh
{
    public class World
    {
        /*
         * Note:
         * This should stay be a per-world property, since we can
         * potentiallly have different tileset for each world.
         */
        private Texture2D tilesetTexture;

        // The interactive tile layer (the only layer for now)
        private Player Player;
        private Tile[] Tiles;
        private int width;
        private int height;

        private HughGame Game;

        public Viewport Viewport { get; set; }

        public int Width
        {
            get => width;
        }

        public int Height
        {
            get => height;
        }

        public World(HughGame game, TmxMap map, int worldId)
        {
            Game = game;
            width = map.Width;
            height = map.Height;
            Tiles = new Tile[width * height];

            var tileset = map.Tilesets[0];
            tilesetTexture = game.Content.Load<Texture2D>(tileset.Name.ToString());

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

                int tilesetTilesWidth = tilesetTexture.Width / Tile.SIZE;

                int tileFrame = gid - 1;
                int column = tileFrame % tilesetTilesWidth;
                int row = tileFrame / tilesetTilesWidth;

                int x = i % width;
                int y = i / width;

                string tileType = GetTilesetTileType(tileset, tileFrame);

                if ("player".Equals(tileType))
                {
                    Player = new Player(row, column, new Vector2(x * Tile.SIZE, y * Tile.SIZE));
                    continue;
                } else {
                    Tiles[y * width + x] = new Tile(row, column, x, y, tileType);
                }
            }
        }

        private string GetTilesetTileType(TmxTileset tileset, int id)
        {
            TmxTilesetTile tilesetTile = TileForId(tileset, id);
            
            if (tilesetTile == null || ! tilesetTile.Properties.ContainsKey("type"))
                return null;

            return tilesetTile.Properties["type"];
        }

        private TmxTilesetTile TileForId(TmxTileset tileset, int id)
        {
            // Terribly inefficient.. we could pretty easily optimize this if loading is too slow
            foreach (TmxTilesetTile t in tileset.Tiles)
            {
                if (t.Id == id)
                    return t;
            }
            return null;
        }

        public void Draw()
        {
            Game.GraphicsDevice.Viewport = Viewport;

            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   null, null, null, null,
                                   GetCameraTransform());

            DrawTiles();

            int playerX = (int)Player.Position.X;
            int playerY = (int)Player.Position.Y;
            var playerPositionRect = new Rectangle(playerX, playerY, Tile.SIZE, Tile.SIZE);
            Game.SpriteBatch.Draw(tilesetTexture, playerPositionRect, Player.TilesetRect, Color.White);

            Game.SpriteBatch.End();
        }

        private Matrix GetCameraTransform()
        {
            int worldWidth = width * Tile.SIZE;
            int worldHeight = height * Tile.SIZE;

            int viewportWidth = Viewport.Width;
            int viewportHeight = Viewport.Height;

            int cameraX;
            int cameraY;

            if (worldWidth <= viewportWidth)
            {
                cameraX = (worldWidth - viewportWidth) / 2;
            }
            else
            {
                int playerCenterX = (int)Player.Position.X + (int)Tile.SIZE / 2;
                cameraX = playerCenterX - viewportWidth / 2;
                cameraX = Math.Max(0, Math.Min(worldWidth - viewportWidth, cameraX));
            }

            if (worldHeight <= viewportHeight)
            {
                cameraY = (worldHeight - viewportHeight) / 2;
            }
            else
            {
                int playerCenterY = (int)Player.Position.Y + (int)Tile.SIZE / 2;
                cameraY = playerCenterY - viewportHeight / 2;
                cameraY = Math.Max(0, Math.Min(worldHeight - viewportHeight, cameraY));
            }

            return Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-cameraX, -cameraY, 0));
        }

        private void DrawTiles()
        {
            for (int i = 0; i < width * height; i++)
            {
                Tile t = Tiles[i];

                if (t == null)
                {
                    continue;
                }

                var positionRect = new Rectangle((int)t.X, (int)t.Y, Tile.SIZE, Tile.SIZE);
                Game.SpriteBatch.Draw(tilesetTexture, positionRect, t.TilesetRect, Color.White);
            }
        }

        public void Update(float dt)
        {
            Player.IsOnFloor = IsPlayerOnFloor();
            Player.Update(dt);

            while (HandleFloorCollisions());

            Player.IsOnFloor = IsPlayerOnFloor();

            Player.Position.X += Player.Velocity.X;
            Player.Position.Y += Player.Velocity.Y;
        }

        // TODO: Move this to the player class, once the multiverse code is in place
        private bool IsPlayerOnFloor()
        {
            int pixelX = (int)Player.Position.X;
            int pixelY = (int)Player.Position.Y;

            if (pixelY % Tile.SIZE != 0)
                return false;

            int tileX = (int)Player.Position.X / Tile.SIZE;
            // The Y coord below the player
            int tileY = (int)Player.Position.Y / Tile.SIZE + 1;

            if (tileY >= height || tileX >= width || tileX < 0 || tileY < 0)
                return false;

            // TODO: Add a NullTile class, so you don't have to do annoying null checks like this

            if (Tiles[tileY * width + tileX] != null && Tiles[tileY * width + tileX].IsGround())
                return true;

            if (pixelX % Tile.SIZE == 0 || tileX + 1 >= width)
                return false;

            if (Tiles[tileY * width + tileX + 1] != null && Tiles[tileY * width + tileX + 1].IsGround())
                return true;

            return false;
        }

        private bool HandleFloorCollisions()
        {
            // Note: If collisions are handled properly, initialRect should never overlap
            Rectangle initialRect = ComputeEntityRect(Player.Position);
            Rectangle finalRect   = ComputeEntityRect(Player.Position + Player.Velocity);

            Rectangle aabb = ComputeAabb(initialRect, finalRect);

            List<Tile> intersectingTiles = GetTilesWithinRect(aabb);

            // TODO: instead of ignoring non-ground tiles, handle them appropriately!
            intersectingTiles = intersectingTiles.FindAll((tile) => tile.IsGround());

            if (intersectingTiles.Count == 0)
            {
                return false;
            }

            // TODO: Just get the minimum manhattan distance, don't bother sorting
            intersectingTiles.Sort((a, b) =>
            {
                // Sort by manhattan distance (faster than euclidean, and "good enough")
                float distA = Math.Abs(a.X - Player.Position.X) + Math.Abs(a.Y - Player.Position.Y);
                float distB = Math.Abs(b.X - Player.Position.X) + Math.Abs(b.Y - Player.Position.Y);

                return (distA > distB) ? 1 : -1;
            });

            Tile t = intersectingTiles[0];

            if (IsVerticalCollision(t))
            {
                if (initialRect.Y < t.Y)
                {
                    // Floor hit
                    Player.Position.Y = (float)Math.Floor(t.Y - Tile.SIZE);
                    Player.Velocity.Y = 0;
                }
                else
                {
                    // Ceiling hit
                    Player.Position.Y = (float)Math.Ceiling(t.Y + Tile.SIZE);
                    Player.Velocity.Y = 0;
                }
            }
            else
            {
                if (initialRect.X < t.X)
                {
                    // Right hit
                    Player.Position.X = (float)Math.Floor(t.X - Tile.SIZE);
                    Player.Velocity.X = 0;
                }
                else
                {
                    // Left hit
                    Player.Position.X = (float)Math.Ceiling(t.X + Tile.SIZE);
                    Player.Velocity.X = 0;
                }
            }

            return true;
        }

        // TODO: make this work for an arbitrary dynamic object
        private bool IsVerticalCollision(Tile t)
        {
            // Comparing floats caused issues with corners
            return (int)Math.Abs(Player.Position.Y - t.Y) >= (int)Math.Abs(Player.Position.X - t.X);
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
                    if (x >= width || x < 0 || y >= height || y < 0)
                    {
                        continue;
                    }

                    Tile tile = Tiles[y * width + x];
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
}
