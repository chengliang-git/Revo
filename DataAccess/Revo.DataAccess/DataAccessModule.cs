using Ninject.Modules;
using Revo.Core.Module;
using Revo.DataAccess.ModuleInfo;

namespace Revo.DataAccess
{
    public class DataAccessModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IModuleInfoProvider>()
                .To<RevoDataAccessModuleInfoProvider>()
                .InSingletonScope();
        }
    }
}
