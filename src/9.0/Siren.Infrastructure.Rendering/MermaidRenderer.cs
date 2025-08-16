using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Siren.Domain;

namespace Siren.Infrastructure.Rendering
{
    public class MermaidRenderer : IDomainRenderer
    {
        private readonly ProgramArguments _programArguments;
        private readonly ILogger<MermaidRenderer> _logger;

        public MermaidRenderer(
            ProgramArguments programArguments,
            ILogger<MermaidRenderer> logger)
        {
            _programArguments = programArguments;
            _logger = logger;
        }

        public StringBuilder Perform(Universe universe)
        {
            _logger.LogInformation("Commencing render to Mermaid syntax");

            var result = new StringBuilder();

            // Text in file replace header
            result.AppendLine(MermaidConstants.SirenAnchorStart);

            // Mermaid header
            result.AppendLine(_programArguments.MermaidBlockBegin ?? MermaidConstants.MermaidBlockBegin);

            // Header
            result.AppendLine($"\t{MermaidConstants.MermaidErDiagramHeader}");
            
            // (optional) neutral theme
            result.AppendLine($"\t{_programArguments.MermaidThemeLine ?? MermaidConstants.MermaidNeutralThemeLine}");

            _logger.LogInformation("Rendered header");

            var entities = universe.Entities.OrderBy(o => o.FullName);

            foreach (var entity in entities)
            {
                // Entity header
                result.AppendLine($"\t{entity.ShortName} {{");

                foreach (var property in entity.Properties)
                {
                    result.Append($"\t\t{property.Type} {property.Name} ");

                    var keys = new List<string>();
                    if (property.IsPrimaryKey)
                        keys.Add("PK");
                    if (property.IsForeignKey)
                        keys.Add("FK");
                    if (property.IsUniqueKey)
                        keys.Add("UK");

                    if (keys.Any())
                    {
                        result.AppendJoin(',', keys);
                    }

                    result.AppendLine();

                    _logger.LogInformation("Rendered entity: {EntityShortName}", entity.ShortName);
                }

                // Entity footer
                result.AppendLine("\t}");
            }

            foreach (var relationship in universe.Relationships)
            {
                result.AppendLine(
                    $"{relationship.Source?.ShortName}"
                    + $"{MapCardinalityToString(relationship.SourceCardinality, true)}--"
                    + $"{MapCardinalityToString(relationship.TargetCardinality, false)}"
                    + $"{relationship.Target?.ShortName} "
                    + ": \"\""
                );
            }

            // Mermaid footer
            result.AppendLine(_programArguments.MermaidBlockEnd ??MermaidConstants.MermaidBlockEnd);

            // Text in file replace footer
            result.AppendLine(MermaidConstants.SirenAnchorEnd);

            _logger.LogInformation("Completed render to Mermaid syntax");

            return result;
        }

        private static string MapCardinalityToString(CardinalityTypeEnum value, bool leftHand)
        {
            switch (value)
            {
                case CardinalityTypeEnum.ZeroOrOne:
                    return leftHand ? "|o" : "o|";
                case CardinalityTypeEnum.ExactlyOne:
                    return "||";
                case CardinalityTypeEnum.ZeroOrMore:
                    return leftHand ? "}o" : "o{";
                case CardinalityTypeEnum.OneOrMore:
                    return leftHand ? "}|" : "|{";
                default:
                    throw new Exception($"No relationship type mapping defined for value {value}");
            }
        }
    }
}