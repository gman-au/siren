using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public class RelationshipFilter : IRelationshipFilter
    {
        private static readonly string[] RelationshipFunctionNames =
        {
            "HasForeignKey",
            "WithMany",
            "HasOne",
            "WithOne",
        };

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Callvirt)
                return false;

            if (instr.Operand is not MethodReference mr)
                return false;

            return RelationshipFunctionNames.Contains(mr.Name);
        }
    }
}
