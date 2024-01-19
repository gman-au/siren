using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Siren.Domain;
using Property = Siren.Domain.Property;

namespace Siren.Infrastructure.Snapshot
{
    public static class SnapshotAssemblyScanner
    {
        public static Universe Perform(Assembly assembly)
        {
            var entities = new List<Entity>();
            var relationships = new List<Relationship>();

            var modelSnapshotTypes =
                assembly
                    .GetTypes()
                    .Where(
                        o => o.BaseType == typeof(ModelSnapshot)
                    )
                    .ToList();

            // Map entities
            foreach (var modelSnapshotType in modelSnapshotTypes)
            {
                var snapshot = (ModelSnapshot)Activator.CreateInstance(modelSnapshotType);
                var entityTypes = snapshot?.Model.GetEntityTypes();

                foreach (var entityType in entityTypes ?? Array.Empty<IEntityType>())
                {
                    var properties = new List<Property>();
                    var entityTypeName = entityType.Name;
                    var tableName = entityTypeName;
                    var tableNameAnnotation =
                        entityType
                            .FindAnnotation("Relational:TableName");

                    if (tableNameAnnotation?.Value is string tableNameAnnotationValue)
                    {
                        tableName = tableNameAnnotationValue;
                    }

                    var entityProperties =
                        entityType
                            .GetProperties();

                    foreach (var entityProperty in entityProperties)
                    {
                        var entityPropertyName = entityProperty.Name;
                        var entityPropertyType = entityProperty.ClrType;
                        
                        // Is it nullable type? 
                        if (Nullable.GetUnderlyingType(entityPropertyType) != null)
                        {
                            // It's nullable
                            entityPropertyType = 
                                Nullable
                                    .GetUnderlyingType(entityPropertyType);
                        }
                        
                        var property = new Property
                        {
                            Name = entityPropertyName,
                            Type = entityPropertyType?.Name,
                            IsPrimaryKey = entityProperty.IsPrimaryKey(),
                            IsForeignKey = entityProperty.IsForeignKey(),
                            IsUniqueKey = entityProperty.IsUniqueIndex()
                        };

                        properties
                            .Add(property);
                    }

                    var entity =
                        new Entity
                        {
                            FullName = entityTypeName,
                            ShortName = tableName,
                            Properties = properties
                        };

                    entities
                        .Add(entity);
                }

                // Map relationships
                foreach (var entityType in entityTypes ?? Array.Empty<IEntityType>())
                {
                    // TODO
                    var foreignKeys = entityType.GetForeignKeys();

                    foreach (var foreignKey in foreignKeys)
                    {
                        var navigation = foreignKey.GetNavigation(true);
                        var sourceType = navigation?.DeclaringEntityType;
                        var targetType = navigation?.TargetEntityType;

                        var sourceName = sourceType?.Name;
                        var targetName = targetType?.Name;

                        var source = entities.FirstOrDefault(o => o.FullName == sourceName);
                        var target = entities.FirstOrDefault(o => o.FullName == targetName);

                        var sourceCardinality = CardinalityTypeEnum.ZeroOrOne;
                        var targetCardinality = CardinalityTypeEnum.ZeroOrOne;

                        var navigationInverse = navigation?.Inverse;

                        if (navigationInverse != null)
                            sourceCardinality =
                                DetermineFromNavigation(
                                    foreignKey.IsRequiredDependent,
                                    navigationInverse
                                );

                        if (navigation != null)
                            targetCardinality =
                                DetermineFromNavigation(
                                    foreignKey.IsRequired,
                                    navigation
                                );

                        if (source != null && target != null)
                        {
                            var relationship = new Relationship
                            {
                                Source = source,
                                Target = target,
                                SourceCardinality = sourceCardinality,
                                TargetCardinality = targetCardinality
                            };

                            relationships
                                .Add(relationship);
                        }
                    }
                }
            }

            var result =
                new Universe
                {
                    Entities = entities,
                    Relationships = relationships
                };

            return result;
        }

        private static CardinalityTypeEnum DetermineFromNavigation(bool required, INavigation navigation)
        {
            var result = CardinalityTypeEnum.ZeroOrOne;

            if (navigation.IsCollection)
            {
                result =
                    required
                        ? CardinalityTypeEnum.OneOrMore
                        : CardinalityTypeEnum.ZeroOrMore;
            }
            else
            {
                result =
                    required
                        ? CardinalityTypeEnum.ExactlyOne
                        : CardinalityTypeEnum.ZeroOrOne;
            }

            return result;
        }
    }
}