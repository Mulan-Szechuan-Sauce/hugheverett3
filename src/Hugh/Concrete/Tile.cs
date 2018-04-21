using Microsoft.Xna.Framework;

namespace Hugh.Concrete
{
    class Tile
    {
        // By design, all tiles are 32x32 px
        public const int SIZE = 32;

        private int Row;
        private int Column;

        private int x;
        private int y;

        private string Type;

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect
        {
            get { return new Rectangle(SIZE * Column, SIZE * Row, SIZE, SIZE); }
        }

        public float X { get => (float)x * SIZE; }
        public float Y { get => (float)y * SIZE; }

        public Tile(int row, int column, int x, int y, string type)
        {
            Row = row;
            Column = column;
            Type = type;
            this.x = x;
            this.y = y;
        }

        public bool IsGround()
        {
            return "ground".Equals(Type);
        }
    }
}
