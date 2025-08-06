namespace Siren.Domain
{
    public class Relationship
    {
        public Entity Source { get; init; }

        public Entity Target { get; init; }

        public CardinalityTypeEnum SourceCardinality { get; init; }

        public CardinalityTypeEnum TargetCardinality { get; init; }
    }
}
