using System;
using Revo.Core.ModuleInfo;

namespace Revo.Infrastructure.EF6.ModuleInfo
{
    internal class RevoInfrastructureEF6ModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.Infrastructure.EF6";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("6BC081BC-7451-482E-AEBE-4230BF47927B");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => false;
    }
}
