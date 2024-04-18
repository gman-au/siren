using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface ITableBuilder
    {
        bool IsApplicable(Instruction instr);

        ExtractedTable Process(Instruction instr);
    }
}