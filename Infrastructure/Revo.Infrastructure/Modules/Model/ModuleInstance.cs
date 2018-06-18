using System;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities.Basic;

namespace Revo.Infrastructure.Modules.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "MOI")]
    public class ModuleInstance : BasicAggregateRoot
    {
        public ModuleInstance(Guid id): base(id)
        {
            
        }

        protected ModuleInstance()
        {
            
        }

        public string InstanceVersion { get; set; }
        public Guid ModuleId { get; set; }
        public Guid InstanceId { get; set; }
    }
}