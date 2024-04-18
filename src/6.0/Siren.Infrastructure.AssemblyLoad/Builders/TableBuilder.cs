using Mono.Cecil;
using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public class TableBuilder : ITableBuilder
    {
        private const string ToTableMethodName = "ToTable";

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Call) return false;

            if (instr.Operand is not MethodReference mr) return false;

            return mr.Name == ToTableMethodName;
        }

        public ExtractedTable Process(Instruction instr)
        {
            var currInstr =
                instr
                    .Previous;

            if (currInstr.OpCode != OpCodes.Ldstr) return null;

            var schemaName =
                currInstr
                    .Operand
                    .ToString();

            currInstr =
                currInstr
                    .Previous;

            if (currInstr.OpCode != OpCodes.Ldstr) return null;

            var tableName =
                currInstr
                    .Operand
                    .ToString();

            var result =
                new ExtractedTable
                {
                    ReferenceInstruction = instr,
                    TableName = tableName,
                    SchemaName = schemaName
                };

            return result;
        }
    }
}