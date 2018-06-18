using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using Revo.Core.Module;
using Revo.Platforms.AspNet.ModuleInfo;

namespace Revo.Platforms.AspNet
{
    public class AspNetPlatformModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoPlatformsAspNetModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
