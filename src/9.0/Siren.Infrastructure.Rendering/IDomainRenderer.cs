using System.Text;
using Siren.Domain;

namespace Siren.Infrastructure.Rendering
{
    public interface IDomainRenderer
    {
        StringBuilder Perform(Universe universe);
    }
}
