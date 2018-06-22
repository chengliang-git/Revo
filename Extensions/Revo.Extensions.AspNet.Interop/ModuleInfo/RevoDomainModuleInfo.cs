using System;
using Revo.Core.ModuleInfo;

namespace Revo.Extensions.AspNet.Interop.ModuleInfo
{
    internal class RevoExtensionsAspNetInteropModuleInfo: BasicModuleInfo
    {
        public const string AspNetInteropModuleName = "Revo.Extensions.AspNet.Interop";
        public const string AspNetInteropModuleVersion = "1.2.0-RC";
        public Guid AspNetInteropModuleId = Guid.Parse("4B952394-43B4-4A64-92B3-24DBB193304D");

        public override string ModuleName => AspNetInteropModuleName;
        public override string ModuleVersion => AspNetInteropModuleVersion;
        public override Guid ModuleId => AspNetInteropModuleId;
        public override bool UsesDatabase => false;
    }
}
