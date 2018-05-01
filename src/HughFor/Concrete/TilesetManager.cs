using HughFor;
using HughFor.Enums;
using TiledSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HughFor.Concrete
{
    public class TilesetManager
    {
        /*
         * TODO: In the future, create a GID lookup table for supa-kwik-n-easy access.
         * We could also drop the TmxList by creating a lookup table!
         */

        private List<Texture2D> TextureList;
        private List<TmxTileset> TmxList;
        private HughFor Game;

        public TilesetManager(HughFor game)
        {
            Game = game;
            TmxList = new List<TmxTileset>();
            TextureList = new List<Texture2D>();
        }

        public void LoadMap(TmxMap map)
        {
            TextureList.Clear();
            TmxList.Clear();

            foreach (var tileset in map.Tilesets)
            {
                var imagePath = tileset.Image.Source;
                // Without the .png extension
                var contentPath = imagePath.Substring(0, imagePath.Length - 4);

                TmxList.Add(tileset);
                TextureList.Add(Game.Content.Load<Texture2D>(contentPath));
            }
        }

        public TileType GetTileType(int gid)
        {
            TmxTileset tileset = TmxList[TilesetIndexForGid(gid)];
            int tileFrame = gid - tileset.FirstGid;

            return GetTilesetTileType(tileset, tileFrame);
        }

        private TileType GetTilesetTileType(TmxTileset tileset, int id)
        {
            TmxTilesetTile tilesetTile = TileForId(tileset, id);

            if (tilesetTile == null || !tilesetTile.Properties.ContainsKey("type"))
                throw new Exception($"Tile {id} has no type.");

            var typeString = tilesetTile.Properties["type"];
            if (Enum.TryParse<TileType>(typeString, true, out var type))
                return type;

            throw new Exception($"Tile type {typeString} is not valid.");
        }

        private TmxTilesetTile TileForId(TmxTileset tileset, int id)
        {
            // Terribly inefficient.. we could pretty easily optimize this if loading is too slow
            foreach (TmxTilesetTile t in tileset.Tiles)
            {
                if (t.Id == id)
                    return t;
            }
            Trace.Assert(false);
            return null;
        }

        // Draws the given gid on the given area
        public void DrawGid(int gid, Rectangle drawArea)
        {
            int tilesetIndex = TilesetIndexForGid(gid);

            TmxTileset tileset = TmxList[tilesetIndex];
            Texture2D texture  = TextureList[tilesetIndex];

            int tileFrame = gid - tileset.FirstGid;

            int columns = tileset.Columns ?? 1;

            int column = tileFrame % columns;
            int row    = tileFrame / columns;

            Rectangle tilesetRect = new Rectangle(tileset.TileWidth * column,
                                                  tileset.TileHeight * row,
                                                  tileset.TileWidth,
                                                  tileset.TileHeight);

            Game.SpriteBatch.Draw(texture, drawArea, tilesetRect, Color.White);
        }

        private int TilesetIndexForGid(int gid)
        {
            // Das right - ignore the first tileset!
            // They *should* be sorted by first GID
            for (int i = 1; i < TmxList.Count; i++)
            {
                if (TmxList[i].FirstGid > gid)
                {
                    return i - 1;
                }
            }

            return TmxList.Count - 1;
        }
    }
}
