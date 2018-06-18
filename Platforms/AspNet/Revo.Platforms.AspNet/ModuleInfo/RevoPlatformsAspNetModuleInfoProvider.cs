using Revo.Core.Module;

namespace Revo.Platforms.AspNet.ModuleInfo
{
    internal class RevoPlatformsAspNetModuleInfoProvider : IModuleInfoProvider
    {
        private static readonly RevoPlatformsAspNetModuleInfo RevoPlatformsAspNetModuleInfo = new RevoPlatformsAspNetModuleInfo();
        public IModuleInfo ModuleInfo => RevoPlatformsAspNetModuleInfo;
    }
}
