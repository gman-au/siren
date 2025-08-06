using System.Collections.Generic;

namespace Siren.Domain
{
    public class Entity
    {
        public string ShortName { get; init; }

        public string FullName { get; init; }

        public IEnumerable<Property> Properties { get; init; }
    }
}
