using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hugh.Concrete
{
    public class Controller
    {
        private static bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

        public static bool IsLeftPressed() => IsKeyDown(Keys.Left) || IsKeyDown(Keys.A);

        public static bool IsRightPressed() => IsKeyDown(Keys.Right) || IsKeyDown(Keys.E);

        public static bool IsUpPressed() => IsKeyDown(Keys.Up) || IsKeyDown(Keys.OemComma);

        public static bool IsDownPressed() => IsKeyDown(Keys.Down) || IsKeyDown(Keys.O);
    }
}
