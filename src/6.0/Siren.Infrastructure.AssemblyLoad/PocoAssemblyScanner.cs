using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Siren.Domain;

namespace Siren.Infrastructure.AssemblyLoad
{
    public static class PocoAssemblyScanner
    {
        public static Universe Perform(Assembly assembly)
        {
            var entities = new List<Entity>();

            var pocos =
                assembly
                    .GetExportedTypes()
                    .Where(
                        o =>
                            o.IsClass
                    )
                    .ToList();

            // Map entities
            foreach (var poco in pocos)
            {
                var name = poco.Name;
                var properties = new List<Property>();

                foreach (var declaredProperty in poco.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var propertyType = declaredProperty.PropertyType;
                    var propertyTypeName = declaredProperty.PropertyType.Name;

                    // Is it nullable type? 
                    if (Nullable.GetUnderlyingType(propertyType) != null)
                    {
                        // It's nullable
                        propertyType = Nullable.GetUnderlyingType(propertyType);
                        propertyTypeName = $"{propertyType?.Name}";
                    }

                    var property = new Property
                    {
                        Name = declaredProperty.Name,
                        Type = propertyTypeName,
                        IsPrimaryKey = ContainsAttribute<KeyAttribute>(declaredProperty),
                        IsForeignKey = ContainsAttribute<ForeignKeyAttribute>(declaredProperty),
                        IsUniqueKey = false
                    };

                    if (!IsVirtual(declaredProperty))
                    {
                        properties
                            .Add(property);
                    }
                }

                var entity =
                    new Entity
                    {
                        ShortName = name,
                        Properties = properties
                    };

                entities
                    .Add(entity);
            }

            var relationships = new List<Relationship>();

            // Map relationships
            foreach (var poco in pocos)
            {
                foreach (var declaredProperty in poco.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (IsVirtual(declaredProperty))
                    {
                        var source = entities.FirstOrDefault(o => o.ShortName == declaredProperty.PropertyType.Name);
                        var target = entities.FirstOrDefault(o => o.ShortName == poco.Name);

                        if (source != null && target != null)
                        {
                            var relationship = new Relationship
                            {
                                Source = source,
                                Target = target,
                                SourceCardinality = CardinalityTypeEnum.ExactlyOne,
                                TargetCardinality = CardinalityTypeEnum.OneOrMore
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

        private static bool ContainsAttribute<T>(MemberInfo property) where T : Attribute
        {
            var customAttributes =
                property
                    .CustomAttributes;

            var exists =
                customAttributes
                    .Any(o => o.AttributeType == typeof(T));

            return exists;
        }

        private static bool IsVirtual(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                return false;
            }

            return
                propertyInfo?
                    .GetGetMethod(true)?
                    .IsVirtual ?? false;
        }
    }
}