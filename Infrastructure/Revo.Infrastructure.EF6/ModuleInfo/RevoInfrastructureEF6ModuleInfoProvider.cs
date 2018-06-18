using Revo.Core.Module;

namespace Revo.Infrastructure.EF6.ModuleInfo
{
    internal class RevoInfrastructureEF6ModuleInfoProvider : IModuleInfoProvider
    {
        private static readonly RevoInfrastructureEF6ModuleInfo RevoInfrastructureEf6ModuleInfo = new RevoInfrastructureEF6ModuleInfo();
        public IModuleInfo ModuleInfo => RevoInfrastructureEf6ModuleInfo;
    }
}
