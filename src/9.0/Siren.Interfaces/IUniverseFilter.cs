using System.Collections.Generic;

namespace Siren.Interfaces;

public interface IUniverseFilter
{
    List<T> FilterEntities<T>(IEnumerable<T> extractedEntities, Domain.ProgramArguments arguments);
}