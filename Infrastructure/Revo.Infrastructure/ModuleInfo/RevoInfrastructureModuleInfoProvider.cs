using Revo.Core.Module;

namespace Revo.Infrastructure.ModuleInfo
{
    internal class RevoInfrastructureModuleInfoProvider : IModuleInfoProvider
    {
        private static readonly RevoInfrastructureModuleInfo AspInteropModuleInfo = new RevoInfrastructureModuleInfo();
        public IModuleInfo ModuleInfo => AspInteropModuleInfo;
    }
}
