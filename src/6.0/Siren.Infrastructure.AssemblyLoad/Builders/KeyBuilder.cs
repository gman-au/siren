using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.AssemblyLoad.Extensions;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public class KeyBuilder : IKeyBuilder
    {
        private const string KeyMethodName = "HasKey";

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Callvirt) return false;

            if (instr.Operand is not MethodReference mr) return false;

            return mr.Name == KeyMethodName;
        }

        public void Process(Instruction instr, IEnumerable<ExtractedProperty> properties)
        {
            var currInstr =
                instr
                    .StepPrevious(2);

            if (currInstr.OpCode != OpCodes.Ldstr) return;

            var propertyName =
                currInstr
                    .Operand
                    .ToString();

            var matchingProperty =
                properties
                    .FirstOrDefault(o => o.PropertyName == propertyName);

            if (matchingProperty != null)
            {
                matchingProperty.IsPrimaryKey = true;
            }
        }
    }
}