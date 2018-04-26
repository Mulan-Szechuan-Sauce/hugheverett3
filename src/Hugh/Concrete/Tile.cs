using Hugh.Enums;
using Microsoft.Xna.Framework;

namespace Hugh.Concrete
{
    public class Tile
    {
        // By design, all tiles are 32x32 px
        public const int SIZE = 32;

        private int Row;
        private int Column;

        private int x;
        private int y;

        public TileType Type;

        public RectangleF Hitbox
        {
            get => new RectangleF(X, Y, SIZE, SIZE);
        }

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect
        {
            get => new Rectangle(SIZE * Column, SIZE * Row, SIZE, SIZE);
        }

        public float X { get => (float)x; }
        public float Y { get => (float)y; }

        public Tile(int row, int column, int mapX, int mapY, TileType type)
        {
            Row = row;
            Column = column;
            Type = type;
            this.x = mapX * SIZE;
            this.y = mapY * SIZE;
        }

        public bool IsGround() => Type == TileType.FlatGround;
    }
}
