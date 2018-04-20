using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hugh {
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
}
