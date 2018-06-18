using Revo.DataAccess.Entities;
using Revo.Domain.Entities.Basic;

namespace Revo.Infrastructure.Modules.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "MOD")]
    public class Module : BasicAggregateRoot
    {
        public string Name { get; set; }

        protected Module()
        {
            
        }
    }
}