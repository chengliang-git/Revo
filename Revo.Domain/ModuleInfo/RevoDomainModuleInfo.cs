using System;
using Revo.Core.ModuleInfo;

namespace Revo.Domain.ModuleInfo
{
    internal class RevoDomainModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.Domain";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("CF54794D-2AE6-49AF-9897-FBEB9F8BB6FA");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => false;
    }
}
