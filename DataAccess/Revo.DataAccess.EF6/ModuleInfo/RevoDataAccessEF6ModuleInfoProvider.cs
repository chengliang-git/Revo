using Revo.Core.Module;
using Revo.Core.ModuleInfo;

namespace Revo.DataAccess.EF6.ModuleInfo
{
    internal class RevoDataAccessEF6ModuleInfoProvider: IModuleInfoProvider
    {
        private static readonly RevoDataAccessEF6ModuleInfo DaEf6ModuleInfo = new RevoDataAccessEF6ModuleInfo();
        public IModuleInfo ModuleInfo => DaEf6ModuleInfo;
    }
}
