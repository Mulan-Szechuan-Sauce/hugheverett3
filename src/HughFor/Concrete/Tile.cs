using HughFor.Enums;
using Microsoft.Xna.Framework;

namespace HughFor.Concrete
{
    public class Tile
    {
        // By design, all tiles are 24x24 px
        public const int SIZE = 24;

        public int TilesetGid { get; private set; }

        private int x;
        private int y;

        public TileType Type;

        public Rectangle RegionToDraw
        {
            get => new Rectangle(x, y, SIZE, SIZE);
        }

        public Tile(int mapX, int mapY, TileType type, int tilesetGid)
        {
            TilesetGid = tilesetGid;
            Type = type;
            this.x = mapX * SIZE;
            this.y = mapY * SIZE;
        }

        public bool IsGround() => Type == TileType.Wall;
    }
}
