using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface IPropertyBuilder
    {
        public bool IsApplicable(Instruction instr);

        public ExtractedProperty Process(Instruction instr);
    }
}