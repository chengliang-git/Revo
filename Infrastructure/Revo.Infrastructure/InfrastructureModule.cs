using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Modules;
using Revo.Core.Module;
using Revo.Infrastructure.ModuleInfo;

namespace Revo.Infrastructure
{
    public class InfrastructureModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoInfrastructureModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
