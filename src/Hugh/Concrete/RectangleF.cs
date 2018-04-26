using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Hugh.Concrete
{
    public class RectangleF
    {
        private Vector2 position, size;

        public Vector2 Position {
            get => position;
            set => position = value;
        }

        public Vector2 Size {
            get => size;
            set => size = value;
        }

        public float X
        {
            get => position.X;
            set => position.X = value;
        }

        public float Y
        {
            get => position.Y;
            set => position.Y = value;
        }

        public float Width
        {
            get => size.X;
            set => size.X = value;
        }

        public float Height
        {
            get => size.Y;
            set => size.Y = value;
        }

        public float Left
        {
            get => position.X;
        }

        public float Right
        {
            get => position.X + size.X;
        }

        public float Top
        {
            get => position.Y;
        }

        public float Bottom
        {
            get => position.Y + size.Y;
        }

        public Vector2 Center
        {
            get => new Vector2(X + Width / 2, Y + Height / 2);
        }

        public RectangleF(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public RectangleF(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public RectangleF(RectangleF rect)
        {
            Position = rect.Position;
            Size = rect.Size;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public static RectangleF Union(RectangleF rectA, RectangleF rectB)
        {
            float x = Math.Min(rectA.X, rectB.X);
            float y = Math.Min(rectA.Y, rectB.Y);

            return new RectangleF(x, y,
                                  Math.Max(rectA.Right,  rectB.Right) - x,
                                  Math.Max(rectA.Bottom, rectB.Bottom) - y);
        }
    }
}
