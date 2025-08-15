using System;
using System.Collections.Generic;
using System.Linq;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Application;

public class UniverseFilter : IUniverseFilter
{
    public Universe FilterEntities(Universe universe, ProgramArguments arguments)
    {
        if (arguments.FilterEntities == null && arguments.SkipEntities == null)
            return universe;

        var filterEntities = LoadCommaSeparatedValues(arguments.FilterEntities);
        var skipEntities = LoadCommaSeparatedValues(arguments.SkipEntities);

        var filteredEntities = universe.Entities
            .Where(e =>
                (
                    !filterEntities.Any() ||
                    filterEntities.Any(f => (e.ShortName ?? e.FullName).Contains(f, StringComparison.OrdinalIgnoreCase))
                ) &&
                !skipEntities.Contains(e.ShortName ?? e.FullName)
            )
            .ToList();

        var filteredEntityNames = filteredEntities
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

    private List<string> LoadCommaSeparatedValues(string values)
    {
        return values?.Split(',')
            .Select(o => o.Trim())
            .Where(o => !string.IsNullOrEmpty(o))
            .ToList() ?? [];
    }
}