using System;
using Revo.Core.ModuleInfo;

namespace Revo.Infrastructure.ModuleInfo
{
    internal class RevoInfrastructureModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.Infrastructure";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("5F08DF74-3C4B-4DBD-AD35-26565239E157");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => true;
    }
}
