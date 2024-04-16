using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Siren.Infrastructure.AssemblyLoad.Configuration;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.AssemblyLoad.Extensions;

namespace Siren.Infrastructure.AssemblyLoad.Builders
{
    public class EntityBuilder : IEntityBuilder
    {
        private const string EntityMethodName = "Entity";
        private readonly IBuildConfigurationProvider _buildConfigurationProvider;
        private readonly IPropertyBuilder _propertyBuilder;
        private readonly ITableBuilder _tableBuilder;

        public EntityBuilder(
            IBuildConfigurationProvider buildConfigurationProvider,
            IPropertyBuilder propertyBuilder, 
            ITableBuilder tableBuilder
        )
        {
            _buildConfigurationProvider = buildConfigurationProvider;
            _propertyBuilder = propertyBuilder;
            _tableBuilder = tableBuilder;
        }

        public bool IsApplicable(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Brtrue_S) return false;

            if (instr.Operand is not Instruction opInstr) return false;

            if (opInstr.OpCode != OpCodes.Callvirt) return false;

            if (opInstr.Operand is not MethodReference mr) return false;

            return mr.Name == EntityMethodName;
        }

        public ExtractedEntity Process(Instruction instr)
        {
            var result =
                new ExtractedEntity
                {
                    ReferenceInstruction = instr,
                    Properties = new List<ExtractedProperty>()
                };

            var configuration =
                _buildConfigurationProvider
                    .Get()
                    .First();

            // Extract name
            var currInstr =
                instr
                    .StepPrevious(configuration.StepsBackToEntityName);

            if (currInstr.OpCode == OpCodes.Ldstr)
            {
                var splitName = 
                    currInstr
                        .Operand
                        .ToString()
                        .SplitNamespace();

                result.EntityName = splitName.Item2;
                result.Namespace = splitName.Item1;
            }

            // Get property builder instruction
            currInstr =
                instr
                    .StepNext(configuration.StepsForwardToPropertyBuilder);

            if (currInstr.OpCode != OpCodes.Ldftn) return result;

            if (currInstr.Operand is not MethodDefinition methodReference) return result;

            // Extract properties
            var propertyInstructions =
                methodReference
                    .Body
                    .Instructions
                    .Where(o => 
                        _propertyBuilder
                            .IsApplicable(o))
                    .ToList();

            var extractedProperties =
                propertyInstructions
                    .Select(o => _propertyBuilder.Process(o))
                    .Where(o => o != null)
                    .ToList();
            
            // Extract table information
            var tableInstr =
                methodReference
                    .Body
                    .Instructions
                    .FirstOrDefault(
                        o =>
                            _tableBuilder
                                .IsApplicable(o)
                    );

            if (tableInstr != null)
            {
                var extractedTable =
                    _tableBuilder
                        .Process(tableInstr);

                if (extractedTable != null)
                {
                    result.SchemaName = extractedTable.SchemaName;
                    result.TableName = extractedTable.TableName;
                }
            }

            if (!extractedProperties.Any()) return null;

            result.Properties = extractedProperties;

            return result;
        }
    }
}