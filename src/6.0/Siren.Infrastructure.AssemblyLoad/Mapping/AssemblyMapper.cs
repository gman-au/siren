using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.AssemblyLoad.Extensions;

namespace Siren.Infrastructure.AssemblyLoad.Mapping
{
    public class AssemblyMapper : IAssemblyMapper
    {
        private readonly ILogger<AssemblyMapper> _logger;

        public AssemblyMapper(ILogger<AssemblyMapper> logger)
        {
            _logger = logger;
        }

        public Universe Map(
            ICollection<ExtractedEntity> extractedEntities, 
            ICollection<ExtractedRelationship> extractedRelationships)
        {
            var entities =
                extractedEntities
                    .Select(
                        o =>
                            new Entity
                            {
                                ShortName = o.EntityName?.ToEscaped(),
                                FullName = o.EntityName,
                                Properties =
                                    o
                                        .Properties
                                        .Select(
                                            p =>
                                                new Property
                                                {
                                                    Name = p.PropertyName?.ToEscaped(),
                                                    Type = p.DataType?.ToEscaped(),
                                                    IsPrimaryKey = false,
                                                    IsForeignKey = p.IsForeignKey,
                                                    IsUniqueKey = false
                                                }
                                        )
                            }
                    )
                    .ToList();

            var relationships =
                extractedRelationships
                    .Select(
                        o =>
                            new Relationship
                            {
                                Source = entities.First(e => e.FullName == o.Source.EntityName),
                                Target = entities.First(e => e.FullName == o.Target.EntityName),
                                SourceCardinality = o.SourceCardinality,
                                TargetCardinality = o.TargetCardinality
                            }
                    )
                    .ToList();

            _logger
                .LogInformation($"{entities.Count} entities mapped from {extractedEntities.Count} extracted");
            
            _logger
                .LogInformation($"{relationships.Count} relationships mapped from {extractedRelationships.Count} extracted");
            
            return new Universe
            {
                Entities = entities,
                Relationships = relationships
            };
        }
    }
}