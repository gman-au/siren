using System;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Siren.Application;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.AssemblyLoad.Builders;
using Siren.Infrastructure.AssemblyLoad.Configuration;
using Siren.Infrastructure.AssemblyLoad.Mapping;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Rendering;
using Siren.Infrastructure.SchemaSearch;
using Siren.Interfaces;

namespace Siren.Tool
{
    public static class Startup
    {
        public static IServiceCollection AddServices()
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<IBuildConfigurationProvider, BuildConfigurationProvider>()
                .AddSingleton<ISirenApplication, SirenApplication>()
                .AddSingleton<IUniverseLoader, AssemblyLoader>()
                .AddSingleton<IUniverseLoader, ConnectionStringLoader>()
                .AddSingleton<IUniverseFilter, UniverseFilter>()
                .AddSingleton<IAssemblyMapper, AssemblyMapper>()
                .AddSingleton<IFileWriter, FileWriter>()
                .AddSingleton<IEntityBuilder, EntityBuilder>()
                .AddSingleton<IPropertyBuilder, PropertyBuilder>()
                .AddSingleton<ITableBuilder, TableBuilder>()
                .AddSingleton<IRelationshipBuilder, RelationshipBuilder>()
                .AddSingleton<IKeyBuilder, KeyBuilder>()
                .AddSingleton<IRelationshipFilter, RelationshipFilter>()
                .AddSingleton<ISearchApplication, SearchApplication>()
                .AddSingleton<IDomainRenderer, MermaidRenderer>()
                .AddSingleton(AddProgramArgumentsFactory());

            services.AddLogging(o => o.AddConsole());

            return services;
        }

        private static Func<string[], IProgramArguments> AddProgramArgumentsFactory()
        {
            return
                args =>
                {
                    var parsedArguments =
                        Parser
                            .Default
                            .ParseArguments<ProgramArguments>(args);

                    var arguments = parsedArguments?.Value ?? new ProgramArguments();

                    arguments.Errors =
                        (parsedArguments?.Errors ?? [])
                        .Select(o => o.ToString());

                    return arguments;
                };
        }
    }
}