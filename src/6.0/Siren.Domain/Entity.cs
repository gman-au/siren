using System.Collections.Generic;

namespace Siren.Domain
{
	public class Entity
	{
		public string Name { get; set; }

		public IEnumerable<Property> Properties { get; set; }
	}
}