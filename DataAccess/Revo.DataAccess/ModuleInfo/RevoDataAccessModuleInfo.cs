using System;
using Revo.Core.ModuleInfo;

namespace Revo.DataAccess.ModuleInfo
{
    internal class RevoDataAccessModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.DataAccess";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("6C77D16C-EE57-4238-8CAD-EFA3142DF6AA");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => false;
    }
}
