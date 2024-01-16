namespace Siren.Domain
{
	public class Relationship
	{
		public Entity Source { get; set; }
		
		public Entity Target { get; set; }
		
		public CardinalityTypeEnum SourceCardinality { get; set; }
		
		public CardinalityTypeEnum TargetCardinality { get; set; }
	}
}