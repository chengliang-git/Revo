using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Modules;
using Revo.Core.Lifecycle;

namespace Revo.Infrastructure.Modules
{
    public class ModulesModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IApplicationStartListener>()
                .To<ModulesRegistrator>()
                .InSingletonScope();
        }
    }
}
