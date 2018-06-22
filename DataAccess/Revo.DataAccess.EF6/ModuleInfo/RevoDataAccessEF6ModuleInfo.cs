using System;
using Revo.Core.ModuleInfo;

namespace Revo.DataAccess.EF6.ModuleInfo
{
    internal class RevoDataAccessEF6ModuleInfo: BasicModuleInfo
    {
        public const string EF6ModuleName = "Revo.DataAccess.EF6";
        public const string EF6ModuleVersion = "1.2.0-RC";
        public Guid EF6ModuleId = Guid.Parse("FC87734A-D15F-44AB-ABC3-D28DBAB3B52C");

        public override string ModuleName => EF6ModuleName;
        public override string ModuleVersion => EF6ModuleVersion;
        public override Guid ModuleId => EF6ModuleId;
        public override bool UsesDatabase => false;
    }
}
