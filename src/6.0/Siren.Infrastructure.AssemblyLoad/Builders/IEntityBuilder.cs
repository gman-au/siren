using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface IEntityBuilder
    {
        bool IsApplicable(Instruction instr);

        ExtractedEntity Process(Instruction instr);
    }
}