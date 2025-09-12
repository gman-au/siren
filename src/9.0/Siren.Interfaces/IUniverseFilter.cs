using Siren.Domain;

namespace Siren.Interfaces
{
    public interface IUniverseFilter
    {
        Universe FilterByEntity(Universe universe);

        Universe FilterBySchema(Universe universe);
    }
}