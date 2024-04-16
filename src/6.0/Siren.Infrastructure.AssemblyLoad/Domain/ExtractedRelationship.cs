using Siren.Domain;

namespace Siren.Infrastructure.AssemblyLoad.Domain
{
    public class ExtractedRelationship
    {
        public ExtractedEntity Source { get; set; }
        
        public ExtractedEntity Target { get; set; }
        
        public CardinalityTypeEnum SourceCardinality { get; set; }

        public CardinalityTypeEnum TargetCardinality { get; set; }
    }
}