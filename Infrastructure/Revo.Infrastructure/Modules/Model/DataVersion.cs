using System;
using System.Collections.Generic;
using System.Text;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities.Basic;

namespace Revo.Infrastructure.Modules.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "DTV")]
    public class DataVersion: BasicAggregateRoot
    {
        public DataVersion(Guid id): base(id)
        {
            
        }

        protected DataVersion()
        {
            
        }

        public string Serie { get; set; }
        public string DataVersionName { get; set; }
    }
}
