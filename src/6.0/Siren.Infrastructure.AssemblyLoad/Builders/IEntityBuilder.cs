using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface IEntityBuilder
    {
        public bool IsApplicable(Instruction instr);

        public ExtractedEntity Process(Instruction instr);
    }
}