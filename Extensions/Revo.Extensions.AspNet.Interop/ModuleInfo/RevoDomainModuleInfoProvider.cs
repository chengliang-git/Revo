using Revo.Core.Module;

namespace Revo.Extensions.AspNet.Interop.ModuleInfo
{
    internal class RevoExtensionsAspNetInteropModuleInfoProvider : IModuleInfoProvider
    {
        private static readonly RevoExtensionsAspNetInteropModuleInfo AspInteropModuleInfo = new RevoExtensionsAspNetInteropModuleInfo();
        public IModuleInfo ModuleInfo => AspInteropModuleInfo;
    }
}
