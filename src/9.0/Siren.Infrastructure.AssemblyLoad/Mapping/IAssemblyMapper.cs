using System.Collections.Generic;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Mapping
{
    public interface IAssemblyMapper
    {
        Universe Map(
            ICollection<ExtractedEntity> extractedEntities,
            ICollection<ExtractedRelationship> extractedRelationships
        );
    }
}
