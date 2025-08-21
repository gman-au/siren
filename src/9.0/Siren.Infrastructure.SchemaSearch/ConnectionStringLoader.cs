using System.Collections.Generic;
using System.Linq;
using SchemaSearch.Domain.Schema;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Infrastructure.SchemaSearch
{
    public class ConnectionStringLoader : IUniverseLoader
    {
        private const string BaseTableType = "BASE TABLE";

        private readonly ISearchApplication _searchApplication;
        private readonly IProgramArguments _programArguments;

        public ConnectionStringLoader(ISearchApplication searchApplication, IProgramArguments programArguments)
        {
            _searchApplication = searchApplication;
            _programArguments = programArguments;
        }

        public bool IsApplicable()
        {
            return !string.IsNullOrEmpty(_programArguments?.ConnectionString);
        }

        public Universe Perform()
        {
            var searchResults = _searchApplication.PerformAsync(_programArguments.ConnectionString).Result;

            var allTables = searchResults.Where(o => o.TableType == BaseTableType).ToList();

            var entities = allTables
                .Select(t => new Entity
                {
                    Schema = t.TableSchema,
                    ShortName = t.TableName,
                    FullName = t.TableName,
                    Properties = t
                        .Columns.OrderBy(o => o.OrdinalPosition)
                        .Select(c => new Property
                        {
                            Name = c.ColumnName,
                            Type = c.DataType,
                            IsPrimaryKey = IsPrimaryKey(t, c, allTables),
                            IsForeignKey = IsForeignKey(t, c, allTables),
                            IsUniqueKey = false,
                        }),
                }).ToList();

            var relationships = BuildRelationships(allTables, entities);

            return new Universe { Entities = entities, Relationships = relationships };
        }

        private static bool IsPrimaryKey(SchemaTable table, SchemaTableColumn column, IEnumerable<SchemaTable> tables)
        {
            var allForeignKeys = tables.SelectMany(o => o.ForeignKeys).ToList();

            var result = allForeignKeys.Any(o =>
                o.ReferencedTableSchema == table.TableSchema
                && o.ReferencedTableName == table.TableName
                && o.ReferencedColumnName == column.ColumnName
            );

            return result;
        }

        private static bool IsForeignKey(SchemaTable table, SchemaTableColumn column, IEnumerable<SchemaTable> tables)
        {
            var allForeignKeys = tables.SelectMany(o => o.ForeignKeys).ToList();

            var result = allForeignKeys.Any(o =>
                o.ForeignKeyTableSchema == table.TableSchema
                && o.ForeignKeyTableName == table.TableName
                && o.ForeignKeyColumnName == column.ColumnName
            );

            return result;
        }

        private static IEnumerable<Relationship> BuildRelationships(
            ICollection<SchemaTable> tables,
            ICollection<Entity> entities
        )
        {
            var results = new List<Relationship>();

            var allForeignKeys = tables.SelectMany(o => o.ForeignKeys).ToList();

            foreach (var foreignKey in allForeignKeys)
            {
                var targetEntity = entities.FirstOrDefault(o =>
                    o.Schema == foreignKey.ReferencedTableSchema &&
                    o.FullName == foreignKey.ReferencedTableName
                    && o.Properties.Any(p => p.Name == foreignKey.ReferencedColumnName)
                );

                var sourceEntity = entities.FirstOrDefault(o =>
                    o.Schema == foreignKey.ForeignKeyTableSchema &&
                    o.FullName == foreignKey.ForeignKeyTableName
                    && o.Properties.Any(p => p.Name == foreignKey.ForeignKeyColumnName)
                );

                if (targetEntity == null || sourceEntity == null)
                    continue;

                var relationship = new Relationship
                {
                    Source = sourceEntity,
                    Target = targetEntity,
                    SourceCardinality = CardinalityTypeEnum.ZeroOrMore,
                    TargetCardinality = CardinalityTypeEnum.ExactlyOne,
                };

                results.Add(relationship);
            }

            return results;
        }
    }
}
