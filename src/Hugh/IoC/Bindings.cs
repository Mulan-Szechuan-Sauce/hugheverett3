using Microsoft.Xna.Framework;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hugh.IoC
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<Game>().To<HughGame>();
        }
    }
}
