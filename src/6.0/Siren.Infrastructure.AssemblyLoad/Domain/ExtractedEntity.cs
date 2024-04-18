using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Siren.Infrastructure.AssemblyLoad.Domain
{
    public class ExtractedEntity
    {
        public Instruction ReferenceInstruction { get; set; }
        
        public string Namespace { get; set; }
        
        public string TableName { get; set; }
        
        public string EntityName { get; set; }
        
        public string SchemaName { get; set; }
        
        public IEnumerable<ExtractedProperty> Properties { get; set; }
    }
}