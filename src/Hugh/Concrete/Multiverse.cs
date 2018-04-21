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
        private List<World> Worlds { get; set; } = new List<World>();
        private string UniverseLayout;

        public Multiverse(HughGame game, TmxMap map)
        {
            UniverseLayout = map.Properties["layout"];

            var layoutHeight = CountChars(UniverseLayout, ';');
            var layoutWidth = CountChars(UniverseLayout, '|') / layoutHeight + 1;
            var numberOfUniverses = layoutHeight * layoutWidth;

            for (int i = 1; i <= numberOfUniverses; i++)
                Worlds.Add(new World(game, map, i));
        }

        public void SetViewport(Viewport viewport)
        {
            var layoutHeight = CountChars(UniverseLayout, ';');
            var layoutWidth = CountChars(UniverseLayout, '|') / layoutHeight + 1;
            var numberOfUniverses = layoutHeight * layoutWidth;

            var x = 0;
            var y = 0;

            for (int i = 0; i < UniverseLayout.Length; i += 2) {
                int worldNumber = UniverseLayout[i] - '0';

                World world = Worlds[worldNumber - 1];

                Viewport worldViewport = viewport;
                worldViewport.Height = viewport.Height / layoutHeight;
                worldViewport.Width  = viewport.Width / layoutWidth;
                worldViewport.Y      = y * worldViewport.Height;
                worldViewport.X      = x * worldViewport.Width;
                world.Viewport = worldViewport;

                Console.WriteLine("" + worldViewport.X);
                Console.WriteLine("" + worldViewport.Y);

                if (UniverseLayout[i + 1] == '|') {
                    x++;
                } else {
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

        public void Update(float dt) => Worlds.ForEach(w => w.Update(dt));

        public void Draw() => Worlds.ForEach(w => w.Draw());
    }
}
