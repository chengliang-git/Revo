using System;
using Revo.Core.ModuleInfo;

namespace Revo.DataAccess.ModuleInfo
{
    internal class RevoDataAccessModuleInfo: BasicModuleInfo
    {
        public const string DataAccessModuleName = "Revo.DataAccess";
        public const string DataAccessModuleVersion = "1.2.0-RC";
        public Guid DataAccessModuleId = Guid.Parse("6C77D16C-EE57-4238-8CAD-EFA3142DF6AA");

        public override string ModuleName => DataAccessModuleName;
        public override string ModuleVersion => DataAccessModuleVersion;
        public override Guid ModuleId => DataAccessModuleId;
        public override bool UsesDatabase => false;
    }
}
