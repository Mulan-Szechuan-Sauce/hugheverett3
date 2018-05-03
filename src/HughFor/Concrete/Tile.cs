using System;
using HughFor.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
                case Direction.East:  return new Point(1, 0);
                case Direction.West:  return new Point(-1, 0);
                case Direction.North: return new Point(0, -1);
                case Direction.South: return new Point(0, 1);
            }
            return Point.Zero;
        }
    }

    public class PlayerTile : Tile
    {
        private enum PlayerState
        {
            Standing,
            WalkingOut,
            WalkingIn,
        }

        public bool HasDied { get => false; }
        public bool HasWon  { get => false; }

        private Direction Direction;
        private Vector2 AnimationOffset;

        private PlayerState State;

        public PlayerTile(HughFor game, int mapX, int mapY, int tilesetGid) :
            base(game, mapX, mapY, tilesetGid, TileType.Player)
        {
            Direction = Direction.None;
            AnimationOffset = new Vector2(0, 0);
            SetState(PlayerState.Standing);
        }

        public override void Update(float dt, World world)
        {
            const float ANIMATION_MOVE_SPEED = SIZE * 2; 

            if (State == PlayerState.Standing)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    TryStartMoving(world, Direction.East);
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    TryStartMoving(world, Direction.West);
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    TryStartMoving(world, Direction.North);
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    TryStartMoving(world, Direction.South);
            }

            if (State == PlayerState.WalkingOut)
            {
                Point dirPoint = PointForDirection(Direction);
                Vector2 dirVector = dirPoint.ToVector2();
                AnimationOffset += dirVector * ANIMATION_MOVE_SPEED * dt;

                if (AnimationOffset.Length() >= SIZE / 2)
                {
                    AnimationOffset -= dirVector * SIZE;
                    SetState(PlayerState.WalkingIn);

                    world.MoveTile(this, X + dirPoint.X, Y + dirPoint.Y);
                }
            }
            else if (State == PlayerState.WalkingIn)
            {
                Vector2 dirVector = PointForDirection(Direction).ToVector2();
                AnimationOffset += dirVector * ANIMATION_MOVE_SPEED * dt;

                // FIXME this will break if lag occurs
                if (Math.Round(AnimationOffset.Length()) == 0)
                {
                    SetState(PlayerState.Standing);
                    AnimationOffset = new Vector2(0, 0);
                }
            }
        }

        private void SetState(PlayerState state)
        {
            State = state;
            Console.WriteLine("Setting state to: " + state.ToString());
        }

        private void TryStartMoving(World world, Direction direction)
        {
            if (CanMoveTowards(world, direction))
            {
                Direction = direction;
                SetState(PlayerState.WalkingOut);
            }
        }

        private bool CanMoveTowards(World world, Direction direction)
        {
            var p = Location + PointForDirection(direction);

            if (p.X <= 0 || p.Y <= 0 || p.X >= world.Width || p.Y >= world.Height)
            {
                return false;
            }
            
            Tile t = world.GetTile(p.X, p.Y);
            
            return t == null || !t.IsWall();
        }

        public override void Draw()
        {
            Rectangle rect = new Rectangle(X * SIZE, Y * SIZE, SIZE, SIZE);

            if (State != PlayerState.Standing)
            {
                rect.X += (int)AnimationOffset.X;
                rect.Y += (int)AnimationOffset.Y;
            }

            Game.TilesetManager.DrawGid(TilesetGid, rect);
        }
    }
}
