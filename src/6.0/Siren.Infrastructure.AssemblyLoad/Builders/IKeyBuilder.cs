using System.Collections.Generic;
using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface IKeyBuilder
    {
        public bool IsApplicable(Instruction instr);

        public void Process(Instruction instr, IEnumerable<ExtractedProperty> properties);
    }
}