using System.Collections.Generic;

namespace Siren.Domain
{
    public class Universe
    {
        public IEnumerable<Entity> Entities { get; init; }

        public IEnumerable<Relationship> Relationships { get; init; }
    }
}
