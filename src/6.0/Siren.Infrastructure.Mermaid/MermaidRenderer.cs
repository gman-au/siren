﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Siren.Domain;

namespace Siren.Infrastructure.Mermaid
{
    public static class MermaidRenderer
    {
        public static StringBuilder Perform(Universe universe)
        {
            var result = new StringBuilder();

            // Text in file replace header
            result
                .AppendLine(MermaidConstants.SirenAnchorStart);

            // Mermaid header
            result
                .AppendLine(MermaidConstants.MermaidAnchorStart);

            // Header
            result
                .AppendLine($"\t{MermaidConstants.MermaidErDiagramHeader}");

            foreach (var entity in universe.Entities)
            {
                // Entity header
                result
                    .AppendLine($"\t{entity.Name} {{");

                foreach (var property in entity.Properties)
                {
                    result
                        .Append($"\t\t{property.Type} {property.Name} ");

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
                    .AppendLine("\t}");
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
                .AppendLine(MermaidConstants.MermaidAnchorEnd);

            // Text in file replace footer
            result
                .AppendLine(MermaidConstants.SirenAnchorEnd);

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