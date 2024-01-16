namespace Siren.Domain
{
	public class Property
	{
		public string Name { get; init; }

		public string Type { get; init; }
		
		public bool IsPrimaryKey { get; init; }
		
		public bool IsForeignKey { get; init; }
		
		public bool IsUniqueKey { get; init; }
	}
}