using System;
using System.Collections.Generic;
using System.Linq;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Infrastructure.AssemblyLoad.Builders;

public class UniverseFilter : IUniverseFilter
{
    public List<T> FilterEntities<T>(IEnumerable<T> extractedEntities, ProgramArguments arguments)
    {
        var filterEntities = LoadCommaSeparatedValues(arguments.FilterEntities);
        var skipEntities = LoadCommaSeparatedValues(arguments.SkipEntities);

        return extractedEntities
            .Where(o => o != null &&
                        (
                            !filterEntities.Any() ||
                            filterEntities.Any(f => GetEntityName(o).Contains(f, StringComparison.OrdinalIgnoreCase))
                        ) &&
                        !skipEntities.Contains(GetEntityName(o))
            )
            .ToList();
    }

    private List<string> LoadCommaSeparatedValues(string values)
    {
        return values?.Split(',')
            .Select(o => o.Trim())
            .Where(o => !string.IsNullOrEmpty(o))
            .ToList() ?? [];
    }

    private string GetEntityName<T>(T entity)
    {
        // Try to get EntityName or ShortName property
        var type = typeof(T);
        var entityNameProp = type.GetProperty("EntityName");
        if (entityNameProp != null)
            return entityNameProp.GetValue(entity)?.ToString() ?? string.Empty;

        var shortNameProp = type.GetProperty("ShortName");
        if (shortNameProp != null)
            return shortNameProp.GetValue(entity)?.ToString() ?? string.Empty;

        return string.Empty;
    }
}