using System;
using System.Collections.Generic;
using System.Linq;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Application
{
    public class UniverseFilter(IProgramArguments programArguments) : IUniverseFilter
    {
        public Universe FilterByEntity(Universe universe)
        {
            if (programArguments?.FilterEntities == null && programArguments?.SkipEntities == null)
                return universe;

            var filterEntities = LoadCommaSeparatedValues(programArguments.FilterEntities);
            var skipEntities = LoadCommaSeparatedValues(programArguments.SkipEntities);

            var filteredEntities =
                universe
                    .Entities
                    .Where(e =>
                        (
                            !filterEntities.Any() ||
                            filterEntities.Any(f => (e.ShortName ?? e.FullName).Contains(f, StringComparison.OrdinalIgnoreCase))
                        ) &&
                        !skipEntities.Contains(e.ShortName ?? e.FullName)
                    )
                    .ToList();

            var filteredEntityNames =
                filteredEntities
                    .Select(e => e.ShortName ?? e.FullName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var filteredRelationships = universe.Relationships?
                .Where(r =>
                    r.Source != null && r.Target != null &&
                    filteredEntityNames.Contains(r.Source.ShortName ?? r.Source.FullName) &&
                    filteredEntityNames.Contains(r.Target.ShortName ?? r.Target.FullName)
                )
                .ToList();

            return new Universe
            {
                Entities = filteredEntities,
                Relationships = filteredRelationships
            };
        }

        public Universe FilterBySchema(Universe universe)
        {
            if (programArguments?.FilterSchemas == null && programArguments?.SkipSchemas == null)
                return universe;

            var filterSchemas = LoadCommaSeparatedValues(programArguments.FilterSchemas);
            var skipSchemas = LoadCommaSeparatedValues(programArguments.SkipSchemas);

            var filteredEntities =
                universe
                    .Entities
                    .Where(e =>
                        (
                            !filterSchemas.Any() ||
                            filterSchemas.Any(f => e.Schema.Contains(f, StringComparison.OrdinalIgnoreCase))
                        ) &&
                        !skipSchemas.Contains(e.Schema)
                    )
                    .ToList();

            var filteredEntityNames =
                filteredEntities
                    .Select(e => e.Schema)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var filteredRelationships =
                universe
                    .Relationships?
                    .Where(r =>
                        r.Source != null && r.Target != null &&
                        filteredEntityNames.Contains(r.Source.Schema) &&
                        filteredEntityNames.Contains(r.Target.Schema)
                    )
                    .ToList();

            return new Universe
            {
                Entities = filteredEntities,
                Relationships = filteredRelationships
            };
        }

        private List<string> LoadCommaSeparatedValues(string values)
        {
            return values?.Split(',')
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrEmpty(o))
                .ToList() ?? [];
        }
    }
}