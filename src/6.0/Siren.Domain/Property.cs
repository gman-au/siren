namespace Siren.Domain
{
	public class Property
	{
		public string Name { get; set; }

		public string Type { get; set; }
		
		public bool IsPrimaryKey { get; set; }
		
		public bool IsForeignKey { get; set; }
		
		public bool IsUniqueKey { get; set; }
	}
}