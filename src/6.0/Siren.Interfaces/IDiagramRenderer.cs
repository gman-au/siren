using System.Text;
using System.Threading.Tasks;
using Siren.Domain;

namespace Siren.Interfaces
{
	public interface IDiagramRenderer
	{
		Task<StringBuilder> RenderAsync(Universe universe);
	}
}