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

        public Multiverse(HughGame game, TmxMap map, int numberOfUniverses)
        {
            for (int i = 1; i <= numberOfUniverses; i++)
                Worlds.Add(new World(game, map, i));
        }

        public World GetWorld(int worldNumber) => Worlds[worldNumber - 1];

        public void Update(float dt) => Worlds.ForEach(w => w.Update(dt));

        public void Draw() => Worlds.ForEach(w => w.Draw());
    }
}
