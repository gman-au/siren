using System.Collections.Generic;

namespace Siren.Domain
{
	public class Entity
	{
		public string Name { get; init; }
		
		public IEnumerable<Property> Properties { get; init; }
	}
}