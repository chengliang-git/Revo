using System;

namespace Revo.Core.ModuleInfo
{
    internal class RevoCoreModuleInfo : BasicModuleInfo
    {
        public const string RevoCoreModuleName = "Revo.Core";
        public const string RevoCoreModuleVersion = "1.1.0";
        public Guid RevoCoreModuleId = Guid.Parse("C89DC8E0-7C33-4665-AEE4-34897E48CB34");

        public override string ModuleName => RevoCoreModuleName;
        public override string ModuleVersion => RevoCoreModuleVersion;
        public override Guid ModuleId => RevoCoreModuleId;
        public override bool UsesDatabase => false;
    }
}
