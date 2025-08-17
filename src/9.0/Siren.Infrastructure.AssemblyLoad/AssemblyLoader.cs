using System.Linq;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad.Builders;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.AssemblyLoad.Mapping;
using Siren.Interfaces;

namespace Siren.Infrastructure.AssemblyLoad
{
    public class AssemblyLoader : IUniverseLoader
    {
        private const string ModelSnapshotBaseType = "ModelSnapshot";
        private const string BuildModelMethod = "BuildModel";

        private readonly IEntityBuilder _entityBuilder;
        private readonly IAssemblyMapper _assemblyMapper;
        private readonly ILogger<AssemblyLoader> _logger;
        private readonly IRelationshipBuilder _relationshipBuilder;
        private readonly IProgramArguments _programArguments;

        public AssemblyLoader(
            ILogger<AssemblyLoader> logger,
            IEntityBuilder entityBuilder,
            IRelationshipBuilder relationshipBuilder,
            IAssemblyMapper assemblyMapper,
            IProgramArguments programArguments
        )
        {
            _logger = logger;
            _entityBuilder = entityBuilder;
            _relationshipBuilder = relationshipBuilder;
            _assemblyMapper = assemblyMapper;
            _programArguments = programArguments;
        }

        public bool IsApplicable()
        {
            return !string.IsNullOrEmpty(_programArguments?.TestAssemblyPath);
        }

        public Universe Perform()
        {
            var filePath = _programArguments.TestAssemblyPath;

            var assembly = AssemblyDefinition.ReadAssembly(filePath);

            foreach (var module in assembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.BaseType?.Name == ModelSnapshotBaseType)
                    {
                        _logger.LogInformation("Located snapshot type {TypeName}", type.Name);

                        foreach (var method in type.Methods)
                        {
                            if (method.Name != BuildModelMethod)
                                continue;

                            _logger.LogInformation("Located build model method");

                            var entityInstructions = method
                                .Body.Instructions.Where(o => _entityBuilder.IsApplicable(o))
                                .ToList();

                            var entities = entityInstructions
                                .Select(o => _entityBuilder.Process(o))
                                .Where(o => o != null)
                                .ToList();

                            _logger.LogInformation("Extracted {EntitiesCount} entities", entities.Count);

                            var relationshipInstructions = method
                                .Body.Instructions.Where(o => _relationshipBuilder.IsApplicable(o))
                                .ToList();

                            var relationships = relationshipInstructions
                                .SelectMany(o => _relationshipBuilder.Process(o, entities)
                                                 // Handle nulls gracefully for relationships among filtered entities
                                                 ?? Enumerable.Empty<ExtractedRelationship>())
                                .ToList();

                            _logger.LogInformation("Extracted {RelationshipsCount} relationships", relationships.Count);

                            var result = _assemblyMapper.Map(entities, relationships);

                            return result;
                        }
                    }
                }
            }

            return null;
        }
    }
}