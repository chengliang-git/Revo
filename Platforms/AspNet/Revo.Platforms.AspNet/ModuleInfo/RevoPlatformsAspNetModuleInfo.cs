using System;
using Revo.Core.ModuleInfo;

namespace Revo.Platforms.AspNet.ModuleInfo
{
    internal class RevoPlatformsAspNetModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.Platforms.AspNet";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("0B66E5A2-009B-4F33-BE50-A6C233EDD1D4");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => false;
    }
}
