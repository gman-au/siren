using Mono.Cecil.Cil;

namespace Siren.Infrastructure.AssemblyLoad.Domain
{
    public class ExtractedProperty
    {
        public Instruction ReferenceInstruction { get; set; }

        public string PropertyName { get; set; }

        public string ColumnName { get; set; }

        public string DataType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }
    }
}
