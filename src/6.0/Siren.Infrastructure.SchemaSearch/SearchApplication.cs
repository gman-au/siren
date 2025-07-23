using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchemaSearch.Domain.Schema;
using SchemaSearch.EntityFramework.Injection;
using SchemaSearch.Interfaces;

namespace Siren.Infrastructure.SchemaSearch
{
    public class SearchApplication : ISearchApplication
    {
        private const string ConnectionStringEnvironmentVariable = "ConnectionStrings__Default";

        public async Task<IList<SchemaTable>> PerformAsync(string connectionString)
        {
            Environment
                .SetEnvironmentVariable(
                    ConnectionStringEnvironmentVariable,
                    connectionString
                );

            var host =
                Host
                    .CreateDefaultBuilder()
                    .ConfigureServices(
                        (context, services) =>
                        {
                            services
                                .AddEntityFrameworkServices(context.Configuration);
                        }
                    )
                    .Build();

            using var scope =
                host
                    .Services
                    .CreateScope();

            var searchApplication =
                scope
                    .ServiceProvider
                    .GetRequiredService<ISchemaSearchApplication>();

            var tables =
                (await
                    searchApplication
                        .RunAsync())
                .ToList();

            await
                host
                    .StopAsync();

            return tables;
        }
    }
}