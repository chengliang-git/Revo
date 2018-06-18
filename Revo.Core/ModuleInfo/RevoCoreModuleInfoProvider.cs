using System;
using System.Collections.Generic;
using System.Text;
using Revo.Core.Module;

namespace Revo.Core.ModuleInfo
{
    internal class RevoCoreModuleInfoProvider: IModuleInfoProvider
    {
        private static readonly RevoCoreModuleInfo CoreModuleInfo = new RevoCoreModuleInfo();
        public IModuleInfo ModuleInfo => CoreModuleInfo;
    }
}
