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
        private readonly IRelationshipFilter _relationshipFilter;

        public RelationshipBuilder(
            IBuildConfigurationProvider buildConfigurationProvider, 
            IRelationshipFilter relationshipFilter)
        {
            _buildConfigurationProvider = buildConfigurationProvider;
            _relationshipFilter = relationshipFilter;
        }

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Brtrue_S) return false;

            if (instr.Operand is not Instruction opInstr) return false;

            if (opInstr.OpCode != OpCodes.Callvirt) return false;

            if (opInstr.Operand is not MethodReference mr) return false;

            return mr.Name == EntityMethodName;
        }

        public ExtractedRelationship Process(Instruction instr, ICollection<ExtractedEntity> entities)
        {
            var configuration =
                _buildConfigurationProvider
                    .Get()
                    .First();
            
            var result = new ExtractedRelationship();
            
            // Extract name
            var currInstr =
                instr
                    .StepPrevious(configuration.StepsBackToEntityName);

            if (currInstr.OpCode != OpCodes.Ldstr) return null;
            
            var extractedSourceName = currInstr.Operand.ToString();

            if (string.IsNullOrEmpty(extractedSourceName)) return null;
            
            var matchedSourceEntity =
                entities
                    .FirstOrDefault(o => o.EntityName == extractedSourceName);

            if (matchedSourceEntity == null) return null;

            result.Source = matchedSourceEntity;

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
                    .Where(
                        o =>
                            _relationshipFilter
                                .IsApplicable(o)
                    )
                    .ToList();

            if (relationshipInstrs.Count > 0)
            {
                foreach (var relationshipInstr in relationshipInstrs)
                {
                    var valueInstr = relationshipInstr.Previous;

                    if (valueInstr.OpCode != OpCodes.Ldstr) continue;

                    // var value = valueInstr.Operand.ToString();
                    
                    if (relationshipInstr.Operand is not MethodReference mr) continue;

                    switch (mr.Name)
                    {
                        case "HasOne":
                            var entityInstr = valueInstr.Previous;
                            if (entityInstr.OpCode != OpCodes.Ldstr) break;
                            var extractedTargetName = entityInstr.Operand.ToString();
                            if (!string.IsNullOrEmpty(extractedTargetName))
                            {
                                var matchedEntity =
                                    entities
                                        .FirstOrDefault(o => 
                                            o.EntityName == extractedTargetName);

                                if (matchedEntity != null)
                                {
                                    result.Target = matchedEntity;
                                }
                            }
                            break;
                        case "HasForeignKey":
                            break;
                        case "WithMany":
                            result.SourceCardinality = CardinalityTypeEnum.ZeroOrOne;
                            result.TargetCardinality = CardinalityTypeEnum.ZeroOrMore;
                            break;
                        case "WithOne":
                            result.SourceCardinality = CardinalityTypeEnum.ZeroOrOne;
                            result.TargetCardinality = CardinalityTypeEnum.ZeroOrOne;
                            break;
                    }
                }
            }

            if (result.Source == null || result.Target == null) return null;
            
            return result;
        }
    }
}