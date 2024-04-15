using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Siren.Application;
using Siren.Infrastructure.AssemblyLoad;
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
                .AddSingleton<ISirenApplication, SirenApplication>()
                .AddSingleton<IAssemblyLoader, AssemblyLoader>()
                .AddSingleton<IFileWriter, FileWriter>()
                .AddSingleton<IDomainRenderer, MermaidRenderer>()
                .AddSingleton<IProgramArgumentsParser, ProgramArgumentsParser>();

            services
                .AddLogging(o => o.AddConsole());
            
            return services;
        } 
    }
}