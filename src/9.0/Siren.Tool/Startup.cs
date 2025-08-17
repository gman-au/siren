using System;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Siren.Application;
using Siren.Domain;
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
        public static IServiceCollection AddServices(string[] args)
        {
            
            var parsedArguments = Parser.Default.ParseArguments<ProgramArguments>(args);

            if (parsedArguments.Errors.Any())
                throw new ArgumentException("Invalid command line arguments provided.");
            var arguments = parsedArguments.Value;
            
            var services = new ServiceCollection();

            services
                .AddSingleton<IProgramArguments, ProgramArguments>()
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
                .AddSingleton(parsedArguments)
                .AddSingleton(arguments);

            services.AddLogging(o => o.AddConsole());

            return services;
        }
    }
}