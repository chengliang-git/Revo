using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using Revo.Core.Module;
using Revo.Infrastructure.EF6.ModuleInfo;

namespace Revo.Infrastructure.EF6
{
    public class InfrastructureEf6Module: NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoInfrastructureEF6ModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
