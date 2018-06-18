using System;
using Revo.Core.ModuleInfo;

namespace Revo.Extensions.AspNet.Interop.ModuleInfo
{
    internal class RevoExtensionsAspNetInteropModuleInfo: BasicModuleInfo
    {
        public const string RevoModuleName = "Revo.Extensions.AspNet.Interop";
        public const string RevoModuleVersion = "1.1.0";
        public Guid RevoModuleId = Guid.Parse("4B952394-43B4-4A64-92B3-24DBB193304D");

        public override string ModuleName => RevoModuleName;
        public override string ModuleVersion => RevoModuleVersion;
        public override Guid ModuleId => RevoModuleId;
        public override bool UsesDatabase => false;
    }
}
