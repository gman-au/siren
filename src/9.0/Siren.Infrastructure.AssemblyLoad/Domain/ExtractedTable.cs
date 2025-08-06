using Mono.Cecil.Cil;

namespace Siren.Infrastructure.AssemblyLoad.Domain
{
    public class ExtractedTable
    {
        public Instruction ReferenceInstruction { get; set; }

        public string TableName { get; set; }

        public string SchemaName { get; set; }
    }
}
