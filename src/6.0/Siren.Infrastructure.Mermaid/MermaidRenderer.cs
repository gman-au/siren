using System.Text;
using System.Threading.Tasks;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Infrastructure.Mermaid
{
	public class MermaidRenderer : IDiagramRenderer
	{
		public async Task<StringBuilder> RenderAsync(Universe universe)
		{
			var result = new StringBuilder();

			// Header
			result.AppendLine("erDiagram");

			foreach (var entity in universe.Entities)
			{
				// Entity opener
				result.AppendLine($"{entity.Name} {{");

				foreach (var property in entity.Properties)
				{
					result.AppendLine($"\t{property.Type} {property.Name}");
				}
				
				// Entity closer
				result.AppendLine("}}");
			}

			return result;
		}
	}
}