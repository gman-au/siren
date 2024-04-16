using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad.Builders;
using Siren.Infrastructure.AssemblyLoad.Mapping;

namespace Siren.Infrastructure.AssemblyLoad
{
    public class AssemblyLoader : IAssemblyLoader
    {
        private readonly IEntityBuilder _entityBuilder;
        private readonly IAssemblyMapper _assemblyMapper;
        private readonly ILogger<AssemblyLoader> _logger;
        private readonly IRelationshipBuilder _relationshipBuilder;

        public AssemblyLoader(
            ILogger<AssemblyLoader> logger,
            IEntityBuilder entityBuilder, 
            IRelationshipBuilder relationshipBuilder, 
            IAssemblyMapper assemblyMapper
        )
        {
            _logger = logger;
            _entityBuilder = entityBuilder;
            _relationshipBuilder = relationshipBuilder;
            _assemblyMapper = assemblyMapper;
        }

        public Universe Perform(ProgramArguments arguments)
        {
            var filePath =
                arguments
                    .TestAssemblyPath;

            var assembly =
                AssemblyDefinition
                    .ReadAssembly(filePath);

            foreach (var module in assembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.BaseType?.Name == "ModelSnapshot")
                    {
                        _logger
                            .LogInformation($"Located snapshot type {type.Name}");

                        foreach (var method in type.Methods)
                        {
                            if (method.Name != "BuildModel") continue;

                            var entityInstructions =
                                method
                                    .Body
                                    .Instructions
                                    .Where(
                                        o =>
                                            _entityBuilder
                                                .IsApplicable(o)
                                    )
                                    .ToList();

                            var entities =
                                entityInstructions
                                    .Select(
                                        o =>
                                            _entityBuilder
                                                .Process(o)
                                    )
                                    .Where(o => o != null)
                                    .ToList();

                            var relationshipInstructions =
                                method
                                    .Body
                                    .Instructions
                                    .Where(
                                        o =>
                                            _relationshipBuilder
                                                .IsApplicable(o)
                                    )
                                    .ToList();

                            var relationships =
                                relationshipInstructions
                                    .SelectMany(
                                        o =>
                                            _relationshipBuilder
                                                .Process(
                                                    o,
                                                    entities
                                                )
                                    )
                                    .Where(o => o != null)
                                    .ToList();

                            var result =
                                _assemblyMapper
                                    .Map(
                                        entities,
                                        relationships
                                    );

                            return result;
                        }
                    }
                }
            }

            return null;
        }
    }
}