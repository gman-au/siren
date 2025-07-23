using System;
using System.Linq;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Infrastructure.SchemaSearch
{
    public class ConnectionStringLoader : IUniverseLoader
    {
        private const string BaseTableType = "BASE TABLE";

        private readonly ISearchApplication _searchApplication;

        public ConnectionStringLoader(ISearchApplication searchApplication)
        {
            _searchApplication = searchApplication;
        }

        public bool IsApplicable(ProgramArguments arguments)
        {
            return !string.IsNullOrEmpty(arguments?.ConnectionString);
        }

        public Universe Perform(ProgramArguments arguments)
        {
            var searchResults =
                _searchApplication
                    .PerformAsync(arguments.ConnectionString)
                    .Result;

            var tables =
                searchResults
                    .Where(o => o.TableType == BaseTableType);

            var entities =
                tables
                    .Select(
                        t => new Entity
                        {
                            ShortName = t.TableName,
                            FullName = t.TableName,
                            Properties =
                                t
                                    .Columns
                                    .OrderBy(o => o.OrdinalPosition)
                                    .Select(
                                        c =>
                                            new Property
                                            {
                                                Name = c.ColumnName,
                                                Type = c.DataType,
                                                IsPrimaryKey = false,
                                                IsForeignKey = false,
                                                IsUniqueKey = false
                                            }
                                    )
                        }
                    );

            return new Universe
            {
                Entities = entities,
                Relationships = Array.Empty<Relationship>()
            };
        }
    }
}