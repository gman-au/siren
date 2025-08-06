using Mono.Cecil.Cil;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface IRelationshipFilter
    {
        bool IsApplicable(Instruction instr);
    }
}
