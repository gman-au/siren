using Siren.Domain;

namespace Siren.Interfaces;

public interface IUniverseFilter
{
    Universe FilterEntities(Universe universe, IProgramArguments arguments);
}