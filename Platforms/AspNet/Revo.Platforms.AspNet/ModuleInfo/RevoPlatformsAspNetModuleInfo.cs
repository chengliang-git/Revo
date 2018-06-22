using System;
using Revo.Core.ModuleInfo;

namespace Revo.Platforms.AspNet.ModuleInfo
{
    internal class RevoPlatformsAspNetModuleInfo: BasicModuleInfo
    {
        public const string AspNetModuleName = "Revo.Platforms.AspNet";
        public const string AspNetModuleVersion = "1.2.0-RC";
        public Guid AspNetModuleId = Guid.Parse("0B66E5A2-009B-4F33-BE50-A6C233EDD1D4");

        public override string ModuleName => AspNetModuleName;
        public override string ModuleVersion => AspNetModuleVersion;
        public override Guid ModuleId => AspNetModuleId;
        public override bool UsesDatabase => false;
    }
}
