using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace Hugh.Concrete
{
    /*
     * TODO: In the future (post-prototype), dynamic objects should be in
     * their own tileset, and be allowed to differ in size.
     */
    class Player
    {
        // By design, the player is 32x32 px (for now?)
        private const int SIZE = 32;

        public Vector2 Position;
        public Vector2 Velocity;
        private int Row;
        private int Column;

        private bool UpWasPressed;

        public bool IsOnFloor { get; set; }

        // The rectangle of tileset to render for this tile
        public Rectangle TilesetRect
        {
            get => new Rectangle(SIZE * Column, SIZE * Row, SIZE, SIZE);
        }

        public bool HasDied { get; set; }
        public bool IsTouchingFinish { get; set; }

        public Player(int row, int column, Vector2 position)
        {
            Row = row;
            Column = column;
            Position = position;
        }

        public void Update(float dt, World world)
        {
            const float ACCELERATION = 7;
            const float MAX_SPEED = 10;
            const float FRICTION = 8f;
            const float GRAVITY = 9.8f; 
            const float JUMP_VEL = 6f;

            if (Controller.isLeftPressed() && !Controller.isRightPressed())
            {
                if (Velocity.X > - MAX_SPEED)
                    Velocity.X = Math.Max(- MAX_SPEED, Velocity.X - ACCELERATION * dt);
            }
            else if (Controller.isRightPressed() && !Controller.isLeftPressed())
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

            if (!UpWasPressed && Controller.isUpPressed() && IsOnFloor)
                Velocity.Y = - JUMP_VEL;

            UpWasPressed = Controller.isUpPressed();

            // Gravity.
            Velocity.Y += GRAVITY * dt;

            HandleObjectCollisions(world);

            // Note: He shouldn't die by touching the ceiling
            if (Position.Y + Tile.SIZE > world.Height * Tile.SIZE ||
                Position.X < 0 ||
                Position.X + Tile.SIZE > world.Width * Tile.SIZE)
                HasDied = true;
        }

        private void HandleObjectCollisions(World world)
        {
            Rectangle hitbox = World.ComputeEntityRect(Position);
            List<Tile> intersectingTiles = world.GetTilesWithinRect(hitbox);
            // Objects count as non-ground tiles
            intersectingTiles = intersectingTiles.FindAll((tile) => !tile.IsGround());

            IsTouchingFinish = false;

            foreach (Tile t in intersectingTiles)
            {
                if (t.Type == "death")
                {
                    HasDied = true;
                }
                else if (t.Type == "finish")
                {
                    IsTouchingFinish = true;
                }
            }
        }

    }
}
