using System;
using Revo.Core.ModuleInfo;

namespace Revo.Infrastructure.EF6.ModuleInfo
{
    internal class RevoInfrastructureEF6ModuleInfo: BasicModuleInfo
    {
        public const string InfrastructureModuleName = "Revo.Infrastructure.EF6";
        public const string InfrastructureModuleVersion = "1.2.0-RC";
        public Guid InfrastructureModuleId = Guid.Parse("6BC081BC-7451-482E-AEBE-4230BF47927B");

        public override string ModuleName => InfrastructureModuleName;
        public override string ModuleVersion => InfrastructureModuleVersion;
        public override Guid ModuleId => InfrastructureModuleId;
        public override bool UsesDatabase => false;
    }
}
