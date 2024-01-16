using System.Reflection;
using System.Threading.Tasks;
using Siren.Domain;

namespace Siren.Interfaces
{
	public interface IAssemblyScanner
	{
		Task<Universe> PerformAsync(Assembly assembly);
	}
}