using System;
using System.Collections.Generic;
using System.Linq;
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

			// Mermaid header
			result
				.AppendLine("```mermaid");

			// Header
			result
				.AppendLine("\terDiagram");

			foreach (var entity in universe.Entities)
			{
				// Entity header
				result
					.AppendLine($"{entity.Name} {{");

				foreach (var property in entity.Properties)
				{
					result
						.Append($"\t{property.Type} {property.Name} ");

					var keys = new List<string>();
					if (property.IsPrimaryKey)
						keys.Add("PK");
					if (property.IsForeignKey)
						keys.Add("FK");
					if (property.IsUniqueKey)
						keys.Add("UK");

					if (keys.Any())
					{
						result
							.AppendJoin(
								',',
								keys
							);
					}

					result
						.AppendLine();
				}

				// Entity footer
				result
					.AppendLine("}");
			}

			foreach (var relationship in universe.Relationships)
			{
				result
					.AppendLine(
						$"{relationship.Source?.Name}" +
						$"{MapCardinalityToString(relationship.SourceCardinality, true)}--" +
						$"{MapCardinalityToString(relationship.TargetCardinality, false)}" +
						$"{relationship.Target?.Name} " +
						": \"\""
					);
			}

			// Mermaid footer
			result
				.AppendLine("```");

			return result;
		}

		private static string MapCardinalityToString(CardinalityTypeEnum value, bool leftHand)
		{
			switch (value)
			{
				case CardinalityTypeEnum.ZeroOrOne: return leftHand ? "|o" : "o|";
				case CardinalityTypeEnum.ExactlyOne: return "||";
				case CardinalityTypeEnum.ZeroOrMore: return leftHand ? "}o" : "o{";
				case CardinalityTypeEnum.OneOrMore: return leftHand ? "}|" : "|{";
				default: throw new Exception($"No relationship type mapping defined for value {value}");
			}
		}
	}
}