using Siren.Domain;

namespace Siren.Interfaces
{
    public interface IUniverseLoader
    {
        bool IsApplicable(IProgramArguments arguments);

        Universe Perform(IProgramArguments arguments);
    }
}
