using HughFor.Concrete;
using HughFor.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using TiledSharp;

namespace HughFor.Concrete
{
    public class World
    {
        // The interactive tile layer (the only layer for now)
        private Tile[] Tiles;
        private int width;
        private int height;

        private HughFor Game;

        public Viewport Viewport { get; set; }

        public int Width
        {
            get => width;
        }

        public int Height
        {
            get => height;
        }

        public World(HughFor game, TmxMap map, int worldId)
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

                Tiles[y * width + x] = new Tile(x, y, tileType, gid);
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

            // FIXME: Focus on the player (once the type exists)
            Vector2 playerCenter = new Vector2(0, 0);

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

            var scale = Matrix.CreateScale(new Vector3(1, 1, 0));
            var translation = Matrix.CreateTranslation(new Vector3(-cameraX, -cameraY, 0));

            return scale * translation;
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

                Game.TilesetManager.DrawGid(t.TilesetGid, t.RegionToDraw);
            }
        }

        public void Update(float dt)
        {
        }

        public List<Tile> GetTilesWithinRect(Rectangle r)
        {
            List<Tile> tiles = new List<Tile>();
            
            int x1 = (int)Math.Floor((float)r.X / Tile.SIZE);
            int x2 = (int)Math.Ceiling(((float)r.X + r.Width) / Tile.SIZE);
            int y1 = (int)Math.Floor((float)r.Y / Tile.SIZE);
            int y2 = (int)Math.Ceiling(((float)r.Y + r.Height) / Tile.SIZE);
            
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

        public Tile GetTile(int x, int y) => Tiles[y * Width + x];
    }
}
