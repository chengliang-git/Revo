using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using Revo.Core.Lifecycle;
using Revo.Core.Module;
using Revo.DataAccess.Entities;
using Revo.Infrastructure.Modules.Model;

namespace Revo.Infrastructure.Modules
{
    public class ModulesRegistrator: IApplicationStartListener
    {
        private readonly IModuleInfoProvider[] moduleInfoProviders;
        private ICrudRepository repository;
        private readonly InstanceInfo instanceInfo;

        public ModulesRegistrator(IModuleInfoProvider[] moduleInfoProviders, ICrudRepository repository, InstanceInfo instanceInfo)
        {
            this.moduleInfoProviders = moduleInfoProviders;
            this.repository = repository;
            this.instanceInfo = instanceInfo;
        }

        public void OnApplicationStarted()
        {
            var dbInstanceInfo = repository.FindAll<Instance>().FirstOrDefault(i => i.Id == instanceInfo.Id);
            if (dbInstanceInfo != null)
                repository.Remove(dbInstanceInfo);

            dbInstanceInfo = new Instance(instanceInfo.Id) {InstanceVersion = instanceInfo.InstanceVersion, Name = instanceInfo.Name};
            repository.Add(dbInstanceInfo);
            repository.SaveChanges();
            foreach (var moduleInfoProvider in moduleInfoProviders.Where(mip => mip.ModuleInfo.UsesDatabase))
            {
                var info = moduleInfoProvider.ModuleInfo;
                var module = repository.FindAll<Module>().First(m => m.Id == info.ModuleId);
                var moduleInstance = new ModuleInstance(Guid.NewGuid()) {InstanceVersion =  info.ModuleVersion, InstanceId = dbInstanceInfo.Id, ModuleId = module.Id};
                repository.Add(moduleInstance);
                repository.SaveChanges();
                var library = new Library(Guid.NewGuid()) {AssemblyName = info.AssemblyName, AssemblyVersion = info.FileVersion, ModuleInstanceId = moduleInstance.Id };
                repository.Add(library);

                foreach (var infoReferencedAssembly in info.ReferencedAssemblies)
                {
                    library = new Library(Guid.NewGuid()) { AssemblyName = infoReferencedAssembly.FullName, ModuleInstanceId = moduleInstance.Id, AssemblyVersion = infoReferencedAssembly.Version.ToString()};
                    repository.Add(library);
                    repository.SaveChanges();
                }
            }
            
            repository.SaveChanges();
        }
    }
}
