using System;
using HughFor.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HughFor.Concrete
{
    public class PlayerTile : Tile
    {
        private enum PlayerState
        {
            Standing,
            Walking,
        }

        public bool HasDied { get; private set; }
        public bool HasWon  { get; private set; }

        private Direction Direction;
        private Vector2 AnimationOffset;

        private PlayerState State;

        public PlayerTile(HughFor game, int mapX, int mapY, int tilesetGid) :
            base(game, mapX, mapY, tilesetGid, TileType.Player)
        {
            Direction = Direction.None;
            AnimationOffset = Vector2.Zero;
            SetState(PlayerState.Standing);

            HasDied = false;
            HasWon = false;
        }

        public override void Update(float dt, World world)
        {
            const float ANIMATION_MOVE_SPEED = SIZE * 4; 

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

            if (State == PlayerState.Walking)
            {
                Point dirPoint = PointForDirection(Direction);
                Vector2 dirVector = dirPoint.ToVector2();
                AnimationOffset += dirVector * ANIMATION_MOVE_SPEED * dt;

                if (AnimationOffset.Length() >= SIZE)
                {
                    AnimationOffset = Vector2.Zero;
                    SetState(PlayerState.Standing);
                    this.X += dirPoint.X;
                    this.Y += dirPoint.Y;
                }
            }

            Tile onTile = world.GetTile(X, Y);
            if (onTile != null)
            {
                HasDied = onTile.Type == TileType.Death;
                HasWon  = onTile.Type == TileType.Finish;
            }
            else
            {
                HasDied = false;
                HasWon  = false;
            }
        }

        private void SetState(PlayerState state)
        {
            State = state;
            //Console.WriteLine(state.ToString() + " " Location.ToString());
        }

        private void TryStartMoving(World world, Direction direction)
        {
            if (CanMoveTowards(world, direction))
            {
                Direction = direction;
                SetState(PlayerState.Walking);
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
