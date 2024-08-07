﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Siren.Application;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.AssemblyLoad.Builders;
using Siren.Infrastructure.AssemblyLoad.Configuration;
using Siren.Infrastructure.AssemblyLoad.Mapping;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Parsing;
using Siren.Infrastructure.Rendering;

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
                .AddSingleton<IAssemblyLoader, AssemblyLoader>()
                .AddSingleton<IAssemblyMapper, AssemblyMapper>()
                .AddSingleton<IFileWriter, FileWriter>()
                .AddSingleton<IEntityBuilder, EntityBuilder>()
                .AddSingleton<IPropertyBuilder, PropertyBuilder>()
                .AddSingleton<ITableBuilder, TableBuilder>()
                .AddSingleton<IRelationshipBuilder, RelationshipBuilder>()
                .AddSingleton<IKeyBuilder, KeyBuilder>()
                .AddSingleton<IRelationshipFilter, RelationshipFilter>()
                .AddSingleton<IDomainRenderer, MermaidRenderer>()
                .AddSingleton<IProgramArgumentsParser, ProgramArgumentsParser>();

            services
                .AddLogging(o => o.AddConsole());
            
            return services;
        } 
    }
}