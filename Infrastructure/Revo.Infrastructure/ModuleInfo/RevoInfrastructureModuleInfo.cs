using System;
using Revo.Core.ModuleInfo;

namespace Revo.Infrastructure.ModuleInfo
{
    internal class RevoInfrastructureModuleInfo: BasicModuleInfo
    {
        public const string InfrastructureModuleName = "Revo.Infrastructure";
        public const string InfrastructureModuleVersion = "1.2.0-RC";
        public Guid InfrastructureModuleId = Guid.Parse("5F08DF74-3C4B-4DBD-AD35-26565239E157");

        public override string ModuleName => InfrastructureModuleName;
        public override string ModuleVersion => InfrastructureModuleVersion;
        public override Guid ModuleId => InfrastructureModuleId;
        public override bool UsesDatabase => true;
    }
}
