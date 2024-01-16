using System.Collections.Generic;

namespace Siren.Domain
{
	public class Universe
	{
		public IEnumerable<Entity> Entities { get; set; }
		
		public IEnumerable<Entity> Relationships { get; set; }
	}
}