using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad.Configuration;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.AssemblyLoad.Extensions;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public class RelationshipBuilder : IRelationshipBuilder
    {
        private const string EntityMethodName = "Entity";

        private readonly IBuildConfigurationProvider _buildConfigurationProvider;

        public RelationshipBuilder(IBuildConfigurationProvider buildConfigurationProvider)
        {
            _buildConfigurationProvider = buildConfigurationProvider;
        }

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Brtrue_S) return false;

            if (instr.Operand is not Instruction opInstr) return false;

            if (opInstr.OpCode != OpCodes.Callvirt) return false;

            if (opInstr.Operand is not MethodReference mr) return false;

            return mr.Name == EntityMethodName;
        }

        public ICollection<ExtractedRelationship> Process(Instruction instr, ICollection<ExtractedEntity> entities)
        {
            var configuration =
                _buildConfigurationProvider
                    .Get()
                    .First();

            var results = new List<ExtractedRelationship>();

            // Extract name
            var currInstr =
                instr
                    .StepPrevious(configuration.StepsBackToEntityName);

            if (currInstr.OpCode != OpCodes.Ldstr) return null;

            var extractedSource =
                currInstr
                    .Operand
                    .ToString()
                    .SplitNamespace();

            if (string.IsNullOrEmpty(extractedSource.Item2)) return null;

            var matchedSourceEntity =
                entities
                    .FirstOrDefault(
                        o =>
                            o.Namespace == extractedSource.Item1 &&
                            o.EntityName == extractedSource.Item2
                    );

            if (matchedSourceEntity == null) return null;

            // Get property builder instruction
            currInstr =
                instr
                    .StepNext(configuration.StepsForwardToPropertyBuilder);

            if (currInstr.OpCode != OpCodes.Ldftn) return null;

            if (currInstr.Operand is not MethodDefinition methodReference) return null;

            // Extract relationship indicators
            var relationshipInstrs =
                methodReference
                    .Body
                    .Instructions
                    .ToList();

            if (relationshipInstrs.Count > 0)
            {
                ExtractedRelationship newResult = null;
                for (var i = 0; i < relationshipInstrs.Count; i++)
                {
                    var mexInstr = relationshipInstrs[i];

                    if (mexInstr.Operand is not MethodReference mr) continue;

                    switch (mr.Name)
                    {
                        case "HasOne":
                            if (newResult != null)
                                results.Add(newResult);

                            newResult = new ExtractedRelationship
                            {
                                Source = matchedSourceEntity
                            };
                            var entityInstr =
                                mexInstr
                                    .StepPrevious(2);

                            if (entityInstr.OpCode != OpCodes.Ldstr) break;

                            var (extractedNamespace, extractedEntityName) =
                                entityInstr
                                    .Operand
                                    .ToString()
                                    .SplitNamespace();

                            if (!string.IsNullOrEmpty(extractedEntityName))
                            {
                                var matchedEntity =
                                    entities
                                        .FirstOrDefault(
                                            o =>
                                                o.EntityName == extractedEntityName &&
                                                o.Namespace == extractedNamespace
                                        );

                                if (matchedEntity != null)
                                {
                                    newResult.Target = matchedEntity;
                                    newResult.TargetCardinality = CardinalityTypeEnum.ZeroOrOne;
                                }
                            }

                            break;
                        case "IsRequired":
                            if (newResult != null)
                                newResult.TargetCardinality =
                                    newResult.TargetCardinality == CardinalityTypeEnum.ZeroOrMore
                                        ? CardinalityTypeEnum.OneOrMore
                                        : CardinalityTypeEnum.ExactlyOne;
                            break;
                        case "HasForeignKey":
                            break;
                        case "WithMany":
                            if (newResult != null)
                                newResult.SourceCardinality = CardinalityTypeEnum.ZeroOrMore;
                            break;
                        case "WithOne":
                            if (newResult != null)
                                newResult.SourceCardinality = CardinalityTypeEnum.ZeroOrOne;
                            break;
                    }
                }

                if (newResult != null)
                    results.Add(newResult);
            }

            results =
                results
                    .Where(
                        o =>
                            o.Source != null &&
                            o.Target != null &&
                            o.SourceCardinality != CardinalityTypeEnum.NotSet &&
                            o.TargetCardinality != CardinalityTypeEnum.NotSet
                    )
                    .ToList();

            return results;
        }
    }
}