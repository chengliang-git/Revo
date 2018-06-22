using System;
using System.Collections.Generic;
using System.Text;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities.Basic;

namespace Revo.Infrastructure.Modules.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "SCL")]
    public class ScriptLog: BasicAggregateRoot
    {
        public ScriptLog(Guid id): base(id)
        {
                
        }

        protected ScriptLog()
        {
            
        }

        public Guid ScriptId { get; set; }
        public string ScriptVersion { get; set; }
        public string ScriptType { get; set; }
        public string ScriptName { get; set;}
        public Guid? DbVersionId { get; set; }
        public Guid? DataVersionId { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime? CompletionTimestamp { get; set; }
    }
}
