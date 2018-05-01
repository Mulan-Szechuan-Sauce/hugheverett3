using Hugh.Concrete;
using Hugh.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using TiledSharp;

namespace Hugh
{
    public class World
    {
        // The interactive tile layer (the only layer for now)
        private Player Player;
        private Tile[] Tiles;
        private int width;
        private int height;

        private HughGame Game;

        public Viewport Viewport { get; set; }

        public bool HasDied
        {
            get => Player.HasDied;
        }

        public bool HasWon
        {
            get => Player.IsTouchingFinish;
        }

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

            AddTilesFromLayer(FindLayer(map, "universal"));
            AddTilesFromLayer(FindLayer(map, string.Format("world{0}", worldId)));
        }

        private TmxLayer FindLayer(TmxMap map, string name)
        {
            foreach (TmxLayer layer in map.Layers)
            {
                if (name.Equals(layer.Name))
                {
                    return layer;
                }
            }
            // Suck it up buttercup
            Trace.Assert(false);
            return null;
        }

        private void AddTilesFromLayer(TmxLayer layer)
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

                int x = i % width;
                int y = i / width;

                var tileType = Game.TilesetManager.GetTileType(gid);

                if (tileType == TileType.Player)
                {
                    Player = new Player(new Vector2(x * Tile.SIZE, y * Tile.SIZE), gid);
                    continue;
                }
                else
                {
                    Tiles[y * width + x] = new Tile(x, y, tileType, gid);
                }
            }
        }

        public void Draw()
        {
            Game.GraphicsDevice.Viewport = Viewport;

            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   null, null, null, null,
                                   GetCameraTransform());

            DrawTiles();

            Game.TilesetManager.DrawGid(Player.TilesetGid, Player.Hitbox.ToRectangle());

            if (Player.HasDied)
            {
                var pos = Player.Position;
                pos.Y -= Player.HEIGHT;
                Game.SpriteBatch.DrawString(Game.GameFont, "R.I.P.", pos, Color.Black);
            }

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

            Vector2 playerCenter = Player.Hitbox.Center;

            if (worldWidth <= viewportWidth)
            {
                cameraX = (worldWidth - viewportWidth) / 2;
            }
            else
            {
                int playerCenterX = (int)playerCenter.X;
                cameraX = playerCenterX - viewportWidth / 2;
                cameraX = Math.Max(0, Math.Min(worldWidth - viewportWidth, cameraX));
            }

            if (worldHeight <= viewportHeight)
            {
                cameraY = (worldHeight - viewportHeight) / 2;
            }
            else
            {
                int playerCenterY = (int)playerCenter.Y;
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
                Game.TilesetManager.DrawGid(t.TilesetGid, positionRect);
            }
        }

        public void Update(float dt)
        {
            if (Player.HasDied) {
                return;
            }
            
            Player.Update(dt, this);

            while (HandleGroundCollisions());

            Player.Position.X += Player.Velocity.X;
            Player.Position.Y += Player.Velocity.Y;
        }

        private bool HandleGroundCollisions()
        {
            // Note: If collisions are handled properly, initialRect should never overlap
            RectangleF initialRect = Player.Hitbox;
            RectangleF finalRect   = OffsetRect(initialRect, Player.Velocity);

            RectangleF aabb = RectangleF.Union(initialRect, finalRect);

            List<Tile> intersectingTiles = GetTilesWithinRect(aabb);

            intersectingTiles = intersectingTiles.FindAll((tile) => tile.IsGround());

            if (intersectingTiles.Count == 0)
            {
                return false;
            }

            Tile t = GetClosestTile(intersectingTiles, Player.Position);

            if (IsVerticalCollision(t))
            {
                if (initialRect.Y < t.Y)
                {
                    // Floor hit
                    Player.Position.Y = (float)Math.Floor(t.Y - Player.HEIGHT);
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
                    Player.Position.X = (float)Math.Floor(t.X - Player.WIDTH);
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

        // Expects a non empty tile list
        private Tile GetClosestTile(List<Tile> tiles, Vector2 origin)
        {
            float minDist = 1e30f;
            Tile minTile = null;

            foreach (Tile t in tiles)
            {
                // Sort by manhattan distance (faster than euclidean, and "good enough")
                float dist = Math.Abs(t.X - origin.X) + Math.Abs(t.Y - origin.Y);

                if (dist < minDist)
                {
                    minTile = t;
                    minDist = dist;
                }
            }

            return minTile;
        }

        // TODO: make this work for an arbitrary dynamic object (in a dynamic object class, really)
        private bool IsVerticalCollision(Tile t)
        {
            // Worst case ontario, this tile will be checked twice!!!
            var newYHitbox = new RectangleF(Player.Hitbox);
            newYHitbox.Y += Player.Velocity.Y;
            return newYHitbox.Intersects(t.Hitbox);
        }

        public List<Tile> GetTilesWithinRect(RectangleF r)
        {
            List<Tile> tiles = new List<Tile>();
            
            int x1 = (int)Math.Floor(r.X / Tile.SIZE);
            int x2 = (int)Math.Ceiling((r.X + r.Width) / Tile.SIZE);
            int y1 = (int)Math.Floor(r.Y / Tile.SIZE);
            int y2 = (int)Math.Ceiling((r.Y + r.Height) / Tile.SIZE);
            
            for (int x = x1; x < x2; x++)
            {
                for (int y = y1; y < y2; y++)
                {
                    if (x >= width || x < 0 || y >= height || y < 0)
                    {
                        continue;
                    }

                    Tile tile = Tiles[y * width + x];
                    if (tile != null && tile.Type != TileType.Empty)
                    {
                        tiles.Add(tile);
                    }
                }
            }
            
            return tiles;
        }

        private static RectangleF OffsetRect(RectangleF rect, Vector2 offset)
        {
            return new RectangleF(rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height);
        }

        public Tile GetTile(int x, int y) => Tiles[y * Width + x];
    }
}
