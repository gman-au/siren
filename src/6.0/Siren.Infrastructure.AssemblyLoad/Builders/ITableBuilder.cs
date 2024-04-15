using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface ITableBuilder
    {
        public bool IsApplicable(Instruction instr);

        public ExtractedTable Process(Instruction instr);
    }
}