using Revo.Core.Module;

namespace Revo.DataAccess.ModuleInfo
{
    internal class RevoDataAccessModuleInfoProvider: IModuleInfoProvider
    {
        private static readonly RevoDataAccessModuleInfo DaModuleInfo = new RevoDataAccessModuleInfo();
        public IModuleInfo ModuleInfo => DaModuleInfo;
    }
}
