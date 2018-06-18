using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Revo.Core.Module
{
    public interface IModuleInfo
    {
        string ModuleName { get; }
        string ModuleVersion { get; }
        Guid ModuleId { get; }
        bool UsesDatabase { get; }

        string AssemblyName { get; }
        string FileName { get; }
        string Version { get; }
        string FileVersion { get; }
        IEnumerable<AssemblyName> ReferencedAssemblies { get; }
    }
}
