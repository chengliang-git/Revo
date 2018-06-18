using System;
using System.Collections.Generic;
using System.Text;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities.Basic;

namespace Revo.Infrastructure.Modules.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "INS")]
    public class Instance: BasicAggregateRoot
    {
        public Instance(Guid id): base(id)
        {
        }

        protected Instance()
        {
            
        }

        public string Name { get; set; }
        public string InstanceVersion { get; set; }
    }
}
