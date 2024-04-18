using System.Collections.Generic;
using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public interface IRelationshipBuilder
    {
        bool IsApplicable(Instruction instr);

        ICollection<ExtractedRelationship> Process(Instruction instr, ICollection<ExtractedEntity> entities);
    }
}