using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace Hugh.Concrete
{
    public class Multiverse
    {
        private Texture2D BorderTexture;
        private List<World> Worlds { get; set; } = new List<World>();
        private string UniverseLayout;
        private int LayoutWidth;
        private int LayoutHeight;
        private Viewport Viewport;
        private HughGame Game;

        public bool HasWon { get; set; }
        public bool HasDied { get; set; }

        public Multiverse(HughGame game, TmxMap map)
        {
            HasWon = false;
            HasDied = false;

            Game = game;
            UniverseLayout = map.Properties["layout"];

            BorderTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            BorderTexture.SetData(new[] { Color.Black });

            LayoutHeight = CountChars(UniverseLayout, ';');
            LayoutWidth = CountChars(UniverseLayout, '|') / LayoutHeight + 1;

            for (int i = 1; i <= LayoutHeight * LayoutWidth; i++)
                Worlds.Add(new World(game, map, i));
        }

        public void SetViewport(Viewport viewport)
        {
            Viewport = viewport;

            var x = 0;
            var y = 0;

            for (int i = 0; i < UniverseLayout.Length; i += 2) {
                int worldNumber = UniverseLayout[i] - '0';

                World world = Worlds[worldNumber - 1];

                Viewport worldViewport = viewport;
                worldViewport.Height = viewport.Height / LayoutHeight;
                worldViewport.Width  = viewport.Width / LayoutWidth;
                worldViewport.Y      = y * worldViewport.Height;
                worldViewport.X      = x * worldViewport.Width;
                world.Viewport = worldViewport;

                if (UniverseLayout[i + 1] == '|') {
                    x++;
                } else {
                    x = 0;
                    y++;
                }
            }
        }

        private int CountChars(string s, char c)
        {
            int count = 0;
            foreach (char i in s) 
                if (i == c) count++;
            return count;
        }

        public World GetWorld(int worldNumber) => Worlds[worldNumber - 1];

        public void Update(float dt)
        {
            if (HasWon || HasDied)
            {
                return;
            }

            bool hasWon = true;

            foreach (World w in Worlds)
            {
                if (w.HasDied)
                {
                    HasDied = true;
                    return;
                }

                // All worlds have to win before the level is complete
                if (!w.HasWon)
                {
                    hasWon = false;
                }
            }

            if (hasWon)
            {
                HasWon = true;
                return;
            }
            
            Worlds.ForEach(w => w.Update(dt));
        }

        public void Draw()
        {
            Worlds.ForEach(w => w.Draw());

            Game.GraphicsDevice.Viewport = Viewport;

            DrawBorders();
        }

        private void DrawBorders()
        {
            Game.SpriteBatch.Begin();

            const int BORDER_WIDTH = 2;

            int width = Viewport.Width / LayoutWidth;
            int height = Viewport.Height / LayoutHeight;

            for (int x = 1; x < LayoutWidth; x++)
            {
                var borderRect = new Rectangle(x * width - BORDER_WIDTH / 2, 0, BORDER_WIDTH, Viewport.Height);
                Game.SpriteBatch.Draw(BorderTexture, borderRect, Color.Black);
            }

            for (int y = 1; y < LayoutHeight; y++)
            {
                var borderRect = new Rectangle(0, y * height - BORDER_WIDTH / 2, Viewport.Width, BORDER_WIDTH);
                Game.SpriteBatch.Draw(BorderTexture, borderRect, Color.Black);
            }

            Game.SpriteBatch.End();
        }
    }
}
