using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

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

        private bool upWasPressed;

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

        public void Update(float dt)
        {
            const float ACCELERATION = 7;
            const float MAX_SPEED = 10;
            const float FRICTION = 8f;
            const float GRAVITY = 9.8f; 
            const float JUMP_VEL = 6f;

            if (Controller.isLeftPressed() && !Controller.isRightPressed())
            {
                if (velocity.X > - MAX_SPEED)
                {
                    velocity.X = Math.Max(- MAX_SPEED, velocity.X - ACCELERATION * dt);
                }
            } else if (Controller.isRightPressed() && !Controller.isLeftPressed()) {
                if (velocity.X < MAX_SPEED)
                {
                    velocity.X = Math.Min(MAX_SPEED, velocity.X + ACCELERATION * dt);
                }
            } else {
                if (velocity.X > 0) {
                    velocity.X = Math.Max(0, velocity.X - FRICTION * dt);
                } else if (velocity.X < 0) {
                    velocity.X = Math.Min(0, velocity.X + FRICTION * dt);
                }
            }

            if (!upWasPressed && Controller.isUpPressed() && this.IsOnFloor)
            {
                velocity.Y = - JUMP_VEL;
            }

            upWasPressed = Controller.isUpPressed();

            // Gravity.
            velocity.Y += GRAVITY * dt;
        }
    }
}
