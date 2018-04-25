using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hugh.Concrete
{
    class Controller
    {
        private static bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

        public static bool isLeftPressed() => IsKeyDown(Keys.Left) || IsKeyDown(Keys.A);

        public static bool isRightPressed() => IsKeyDown(Keys.Right) || IsKeyDown(Keys.E);

        public static bool isUpPressed() => IsKeyDown(Keys.Up) || IsKeyDown(Keys.OemComma);

        public static bool isDownPressed() => IsKeyDown(Keys.Down) || IsKeyDown(Keys.O);
    }
}
