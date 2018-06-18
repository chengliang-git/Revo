using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Modules;
using Revo.Core.Module;
using Revo.Domain.ModuleInfo;

namespace Revo.Domain
{
    public class DomainModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoDomainModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
