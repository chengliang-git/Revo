using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using Revo.Core.Module;
using Revo.Extensions.AspNet.Interop.ModuleInfo;

namespace Revo.Extensions.AspNet.Interop
{
    public class AspNetInteropModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoExtensionsAspNetInteropModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
