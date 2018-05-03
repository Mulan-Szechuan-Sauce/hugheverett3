using HughFor.Enums;
using Microsoft.Xna.Framework;

namespace HughFor.Concrete
{
    public class Tile
    {
        // By design, all tiles are 24x24 px
        public const int SIZE = 24;

        public int TilesetGid { get; private set; }

        // The coordinates in the map of this tile.
        // NOTE: Make sure you update the World grid if you change these
        public int X;
        public int Y;

        public TileType Type;

        protected HughFor Game;

        public Point Location {
            get => new Point(X, Y);
        }

        public Tile(HughFor game, int mapX, int mapY, int tilesetGid, TileType type)
        {
            Game = game;
            TilesetGid = tilesetGid;
            Type = type;
            this.X = mapX;
            this.Y = mapY;
        }
    
        public virtual bool IsWall() => Type == TileType.Wall;

        public virtual void Update(float dt, World world)
        {
        }

        public virtual void Draw()
        {
            var rect = new Rectangle(X * SIZE, Y * SIZE, SIZE, SIZE);
            Game.TilesetManager.DrawGid(TilesetGid, rect);
        }

        protected Point PointForDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:  return new Point(1, 0);
                case Direction.West:  return new Point(-1, 0);
                case Direction.North: return new Point(0, -1);
                case Direction.South: return new Point(0, 1);
            }
            return Point.Zero;
        }
    }
}
