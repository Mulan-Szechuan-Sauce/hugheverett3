using Hugh.Concrete;
using Hugh.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hugh.Concrete
{
    /*
     * TODO: In the future (post-prototype), dynamic objects should be in
     * their own tileset, and be allowed to differ in size.
     */
    class Player
    {
        // By design, the player hitbox is 32x48 px (FOREVER)
        public const int WIDTH = 32;
        public const int HEIGHT = 48;

        public Vector2 Position;
        public Vector2 Velocity;

        public int TilesetGid { get; private set; }

        private bool UpWasPressed;

        // TODO: Draw using the dynamic object / player tileset, not the world tileset!

        public RectangleF Hitbox
        {
            get => new RectangleF(Position.X, Position.Y, WIDTH, HEIGHT);
        }

        public bool HasDied { get; set; }
        public bool IsTouchingFinish { get; set; }

        public Player(Vector2 position, int tilesetGid)
        {
            TilesetGid = tilesetGid;
            Position = position;
        }

        public void Update(float dt, World world)
        {
            const float MAX_SPEED = 5;
            // Accelerate to max speec in 0.5 seconds
            const float ACCELERATION = MAX_SPEED * 2;
            const float FRICTION = ACCELERATION;
            const float GRAVITY = 16f;
            const float JUMP_VEL = 7.8f;

            if (Controller.IsLeftPressed() && !Controller.IsRightPressed())
            {
                if (Velocity.X > - MAX_SPEED)
                    Velocity.X = Math.Max(- MAX_SPEED, Velocity.X - ACCELERATION * dt);
            }
            else if (Controller.IsRightPressed() && !Controller.IsLeftPressed())
            {
                if (Velocity.X < MAX_SPEED)
                    Velocity.X = Math.Min(MAX_SPEED, Velocity.X + ACCELERATION * dt);
            }
            else
            {
                if (Velocity.X > 0)
                    Velocity.X = Math.Max(0, Velocity.X - FRICTION * dt);
                else if (Velocity.X < 0)
                    Velocity.X = Math.Min(0, Velocity.X + FRICTION * dt);
            }

            if (!UpWasPressed && Controller.IsUpPressed() && IsPlayerOnFloor(world))
                Velocity.Y = - JUMP_VEL;

            UpWasPressed = Controller.IsUpPressed();

            // Gravity.
            Velocity.Y += GRAVITY * dt;

            HandleObjectCollisions(world);

            const int EDGE_BUF = 5 * Tile.SIZE;

            // Note: He shouldn't die by touching the ceiling
            if (Position.Y + HEIGHT > world.Height * Tile.SIZE ||
                Position.X < -EDGE_BUF ||
                Position.X + WIDTH > world.Width * Tile.SIZE + EDGE_BUF)
                HasDied = true;
        }

        private void HandleObjectCollisions(World world)
        {
            List<Tile> intersectingTiles = world.GetTilesWithinRect(Hitbox);
            // Objects count as non-ground tiles
            intersectingTiles = intersectingTiles.FindAll((tile) => !tile.IsGround());

            IsTouchingFinish = false;

            foreach (Tile t in intersectingTiles)
            {
                if (t.Type == TileType.Death)
                {
                    HasDied = true;
                }
                else if (t.Type == TileType.Finish)
                {
                    IsTouchingFinish = true;
                }
            }
        }

        public bool IsPlayerOnFloor(World world)
        {
            var extendedHitbox = new RectangleF(Hitbox);
            extendedHitbox.Height = extendedHitbox.Height + 1;

            var area = world.GetTilesWithinRect(extendedHitbox);
            return area.Any(o => o.IsGround());
        }
    }
}
