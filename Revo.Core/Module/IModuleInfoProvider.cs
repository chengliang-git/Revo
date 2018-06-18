using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Core.Module
{
    public interface IModuleInfoProvider
    {
        IModuleInfo ModuleInfo { get; }
    }
}
