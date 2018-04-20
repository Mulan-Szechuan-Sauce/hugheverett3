using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hugh {
    class Controller
    {
        private static bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static bool isLeftPressed()
        {
            return IsKeyDown(Keys.Left) || IsKeyDown(Keys.A);
        }

        public static bool isRightPressed()
        {
            return IsKeyDown(Keys.Right) || IsKeyDown(Keys.E);
        }

        public static bool isUpPressed()
        {
            return IsKeyDown(Keys.Up) || IsKeyDown(Keys.OemComma);
        }

        public static bool isDownPressed()
        {
            return IsKeyDown(Keys.Down) || IsKeyDown(Keys.O);
        }
    }
}
