using Revo.Core.Module;

namespace Revo.Domain.ModuleInfo
{
    internal class RevoDomainModuleInfoProvider : IModuleInfoProvider
    {
        private static readonly RevoDomainModuleInfo DomainModuleInfo = new RevoDomainModuleInfo();
        public IModuleInfo ModuleInfo => DomainModuleInfo;
    }
}
