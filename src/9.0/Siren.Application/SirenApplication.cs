using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Siren.Domain;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Rendering;
using Siren.Interfaces;

namespace Siren.Application
{
    public class SirenApplication : ISirenApplication
    {
        private readonly IDomainRenderer _domainRenderer;
        private readonly IFileWriter _fileWriter;
        private readonly ILogger<SirenApplication> _logger;
        private readonly IEnumerable<IUniverseLoader> _universeLoaders;
        private readonly IUniverseFilter _universeFilter;
        private readonly IProgramArguments _programArguments;

        public SirenApplication(
            ILogger<SirenApplication> logger,
            IFileWriter fileWriter,
            IDomainRenderer domainRenderer,
            IEnumerable<IUniverseLoader> universeLoaders,
            IUniverseFilter universeFilter,
            IProgramArguments programArguments
        )
        {
            _logger = logger;
            _fileWriter = fileWriter;
            _domainRenderer = domainRenderer;
            _universeLoaders = universeLoaders;
            _universeFilter = universeFilter;
            _programArguments = programArguments;
        }

        public int Perform(string[] args)
        {
            try
            {
                _logger.LogInformation("Starting Siren console...");

                var errors = _programArguments.Init(args);
                if (errors.Any())
                    return -1;

                var outputPath = _programArguments.OutputFilePath;
                var markdownAnchor = _programArguments.MarkdownAnchor;

                var assemblyProvided = !string.IsNullOrWhiteSpace(_programArguments.TestAssemblyPath);
                var connectionStringProvided = !string.IsNullOrWhiteSpace(_programArguments.ConnectionString);

                if (assemblyProvided && connectionStringProvided)
                    throw new Exception("Specify one of either test assembly path or connection string.");

                var universeLoader = _universeLoaders.FirstOrDefault(o => o.IsApplicable());

                if (universeLoader == null)
                    throw new Exception(
                        "An error was encountered; no data loading was performed based on the arguments provided."
                    );

                var universe = universeLoader.Perform();
                var filteredUniverse = _universeFilter.FilterEntities(universe);

                var result = _domainRenderer.Perform(filteredUniverse);

                _fileWriter.Perform(outputPath, result, markdownAnchor);

                _logger.LogInformation("Siren operation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error encountered: {ExMessage}", ex.Message);

                return -2;
            }

            return 0;
        }
    }
}
