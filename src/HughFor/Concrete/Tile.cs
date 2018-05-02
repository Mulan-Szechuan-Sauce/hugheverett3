using HughFor.Concrete;
using HughFor.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

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

        public Tile(HughFor game, int mapX, int mapY, int tilesetGid, TileType type)
        {
            Game = game;
            TilesetGid = tilesetGid;
            Type = type;
            this.X = mapX;
            this.Y = mapY;
        }
    
        public static Tile CreateTile(HughFor game, int mapX, int mapY, int tilesetGid, TileType type)
        {
            if (type == TileType.Player)
            {
                return new PlayerTile(game, mapX, mapY, tilesetGid);
            }
            return new Tile(game, mapX, mapY, tilesetGid, type);
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
                case Direction.East:  return new Point(X + 1, Y);
                case Direction.West:  return new Point(X - 1, Y);
                case Direction.North: return new Point(X, Y - 1);
                case Direction.South: return new Point(X, Y + 1);
            }
            return Point.Zero;
        }
    }

    public class PlayerTile : Tile
    {
        public bool HasDied { get => false; }
        public bool HasWon  { get => false; }

        private Direction Direction;
        private Vector2 AnimationOffset;

        public PlayerTile(HughFor game, int mapX, int mapY, int tilesetGid) :
            base(game, mapX, mapY, tilesetGid, TileType.Player)
        {
            Direction = Direction.None;
            AnimationOffset = Vector2.Zero;
        }

        public override void Update(float dt, World world)
        {
            const float ANIMATION_MOVE_SPEED = SIZE * 2; 

            if (Direction == Direction.None)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    TryMoving(world, Direction.East);
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    TryMoving(world, Direction.West);
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    TryMoving(world, Direction.North);
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    TryMoving(world, Direction.South);
                else
                    AnimationOffset = Vector2.Zero;
            }
            else
            {
                // TODO Clean up this mess

                if (Direction == Direction.East)
                {
                    bool hasMoved = AnimationOffset.X < 0;
                    AnimationOffset.X += (float)ANIMATION_MOVE_SPEED * dt;

                    if (hasMoved)
                    {
                        // Animation has finished
                        if (AnimationOffset.X >= 0)
                        {
                            Direction = Direction.None;
                            AnimationOffset = Vector2.Zero;
                        }
                    }
                    else
                    {
                        if (AnimationOffset.X > SIZE / 2)
                        {
                            AnimationOffset.X -= (float)SIZE;
                            var dirPoint = PointForDirection(Direction);
                            world.MoveTile(this, dirPoint.X, dirPoint.Y);
                        }
                    }
                }
                else if (Direction == Direction.West)
                {
                    bool hasMoved = AnimationOffset.X > 0;
                    AnimationOffset.X -= (float)ANIMATION_MOVE_SPEED * dt;

                    if (hasMoved)
                    {
                        // Animation has finished
                        if (AnimationOffset.X <= 0)
                        {
                            Direction = Direction.None;
                            AnimationOffset = Vector2.Zero;
                        }
                    }
                    else
                    {
                        if (AnimationOffset.X < - SIZE / 2)
                        {
                            AnimationOffset.X += (float)SIZE;
                            var dirPoint = PointForDirection(Direction);
                            world.MoveTile(this, dirPoint.X, dirPoint.Y);
                        }
                    }
                }
            }
        }

        private void TryMoving(World world, Direction direction)
        {
            if (CanMoveTowards(world, direction))
                Direction = direction;
        }

        private bool CanMoveTowards(World world, Direction direction)
        {
            var p = PointForDirection(direction);

            if (p.X <= 0 || p.Y <= 0 || p.X >= world.Width || p.Y >= world.Height)
            {
                return false;
            }
            
            Tile t = world.GetTile(p.X, p.Y);
            
            return t == null || !t.IsWall();
        }

        public override void Draw()
        {
            var rect = new Rectangle(X * SIZE + (int)AnimationOffset.X,
                                     Y * SIZE + (int)AnimationOffset.Y,
                                     SIZE, SIZE);
            Game.TilesetManager.DrawGid(TilesetGid, rect);
        }
    }
}
