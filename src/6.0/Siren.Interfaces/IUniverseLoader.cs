using Siren.Domain;

namespace Siren.Interfaces
{
    public interface IUniverseLoader
    {
        bool IsApplicable(ProgramArguments arguments);
        
        Universe Perform(ProgramArguments arguments);
    }
}