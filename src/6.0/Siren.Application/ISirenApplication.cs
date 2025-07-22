using Siren.Domain;

namespace Siren.Application
{
    public interface ISirenApplication
    {
        int Perform(string[] args);
    }
}