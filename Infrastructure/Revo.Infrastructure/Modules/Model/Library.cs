using System;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities.Basic;

namespace Revo.Infrastructure.Modules.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "LIB")]
    public class Library : BasicAggregateRoot
    {
        public Library(Guid id): base(id)
        {
            
        }

        protected Library()
        {
            
        }

        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }
        public Guid ModuleInstanceId { get; set; }
    }
}