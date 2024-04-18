using Siren.Domain;

namespace Siren.Infrastructure.AssemblyLoad
{
    public interface IAssemblyLoader
    {
        Universe Perform(ProgramArguments arguments);
    }
}