using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Modules;
using Revo.Core.Module;
using Revo.Core.ModuleInfo;

namespace Revo.Core
{
    public class RevoCoreModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoCoreModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
