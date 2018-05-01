using Hugh.Enums;
using Microsoft.Xna.Framework;

namespace Hugh.Concrete
{
    public class Tile
    {
        // By design, all tiles are 32x32 px
        public const int SIZE = 32;

        public int TilesetGid { get; private set; }

        private int x;
        private int y;

        public TileType Type;

        public RectangleF Hitbox
        {
            get => new RectangleF(X, Y, SIZE, SIZE);
        }

        public float X { get => (float)x; }
        public float Y { get => (float)y; }

        public Tile(int mapX, int mapY, TileType type, int tilesetGid)
        {
            TilesetGid = tilesetGid;
            Type = type;
            this.x = mapX * SIZE;
            this.y = mapY * SIZE;
        }

        public bool IsGround() => Type == TileType.FlatGround;
    }
}
