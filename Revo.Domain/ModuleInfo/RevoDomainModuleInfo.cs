using System;
using Revo.Core.ModuleInfo;

namespace Revo.Domain.ModuleInfo
{
    internal class RevoDomainModuleInfo: BasicModuleInfo
    {
        public const string DomainModuleName = "Revo.Domain";
        public const string DomainModuleVersion = "1.2.0-RC";
        public Guid DomainModuleId = Guid.Parse("CF54794D-2AE6-49AF-9897-FBEB9F8BB6FA");

        public override string ModuleName => DomainModuleName;
        public override string ModuleVersion => DomainModuleVersion;
        public override Guid ModuleId => DomainModuleId;
        public override bool UsesDatabase => false;
    }
}
