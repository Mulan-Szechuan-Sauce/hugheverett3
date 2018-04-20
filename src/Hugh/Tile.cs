using Microsoft.Xna.Framework;

namespace Hugh
{
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
}
