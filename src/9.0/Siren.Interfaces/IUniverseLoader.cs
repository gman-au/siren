using Siren.Domain;

namespace Siren.Interfaces
{
    public interface IUniverseLoader
    {
        bool IsApplicable();

        Universe Perform();
    }
}
