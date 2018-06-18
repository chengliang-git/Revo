using System;
using Revo.Core.ModuleInfo;

namespace Revo.DataAccess.EF6.ModuleInfo
{
    internal class RevoDataAccessEF6ModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.DataAccess.EF6";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("FC87734A-D15F-44AB-ABC3-D28DBAB3B52C");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => false;
    }
}
