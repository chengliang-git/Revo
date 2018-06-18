using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Revo.Core.Module;

namespace Revo.Core.ModuleInfo
{
    public abstract class BasicModuleInfo: IModuleInfo
    {
        public virtual string ModuleName { get; protected set; }
        public virtual string ModuleVersion { get; protected set; }
        public abstract bool UsesDatabase { get; }
        public virtual string AssemblyName { get; protected set; }
        public virtual string FileName { get; protected set; }
        public virtual string Version { get; protected set; }
        public virtual string FileVersion { get; protected set; }
        public virtual IEnumerable<AssemblyName> ReferencedAssemblies { get; protected set; }
        public virtual Guid ModuleId { get; protected set; }


        protected BasicModuleInfo()
        {
            var assembly = Assembly.GetAssembly(GetType());
            AssemblyName = assembly.FullName;
            FileName = assembly.Location;
            Version = assembly.GetName().Version.ToString();
            FileVersion = FileVersionInfo.GetVersionInfo(FileName).ToString();
            ReferencedAssemblies = assembly.GetReferencedAssemblies().Select(ra => ra).ToList();
        }

    }
}
