using Mono.Cecil;
using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.AssemblyLoad.Extensions;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public class PropertyBuilder : IPropertyBuilder
    {
        private const string PropertyMethodName = "Property";

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Callvirt)
                return false;

            if (instr.Operand is not MethodReference mr)
                return false;

            return mr.Name == PropertyMethodName;
        }

        public ExtractedProperty Process(Instruction instr)
        {
            var result = new ExtractedProperty { ReferenceInstruction = instr };

            var currInstr = instr.StepPrevious(1);

            if (currInstr.OpCode != OpCodes.Ldstr)
                return null;

            var propertyName = currInstr.Operand.ToString();

            currInstr = instr;

            while (currInstr.OpCode != OpCodes.Pop)
            {
                currInstr = currInstr.StepNext(1);

                if (currInstr.OpCode == OpCodes.Callvirt)
                    continue;

                if (currInstr.OpCode != OpCodes.Call)
                    continue;

                var valueInstr = currInstr.Previous;

                if (valueInstr.OpCode != OpCodes.Ldstr)
                    continue;

                var value = valueInstr.Operand.ToString();

                if (currInstr.Operand is not MethodReference mr)
                    continue;

                switch (mr.Name)
                {
                    case "HasColumnName":
                        result.ColumnName = value;
                        break;
                    case "HasColumnType":
                        result.DataType = value;
                        break;
                }
            }

            result.PropertyName = propertyName;

            if (string.IsNullOrEmpty(result.DataType))
                return null;

            return result;
        }
    }
}
