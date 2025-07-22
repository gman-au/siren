using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Logging;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad;
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

        public SirenApplication(
            ILogger<SirenApplication> logger,
            IFileWriter fileWriter,
            IDomainRenderer domainRenderer,
            IEnumerable<IUniverseLoader> universeLoaders
        )
        {
            _logger = logger;
            _fileWriter = fileWriter;
            _domainRenderer = domainRenderer;
            _universeLoaders = universeLoaders;
        }

        public int Perform(string[] args)
        {
            try
            {
                _logger
                    .LogInformation("Starting Siren console...");

                var parsedArguments =
                    Parser
                        .Default
                        .ParseArguments<ProgramArguments>(args);

                if (parsedArguments.Errors.Any())
                    return -1;

                var arguments = parsedArguments.Value;

                var outputPath = arguments.OutputFilePath;
                var markdownAnchor = arguments.MarkdownAnchor;

                var assemblyProvided = !string.IsNullOrWhiteSpace(arguments.TestAssemblyPath);
                var connectionStringProvided = !string.IsNullOrWhiteSpace(arguments.ConnectionString);

                if (assemblyProvided && connectionStringProvided)
                    throw new Exception("Specify one of either test assembly path or connection string.");

                var universeLoader =
                    _universeLoaders
                        .FirstOrDefault(o => o.IsApplicable(arguments));

                if (universeLoader == null)
                    throw new Exception("An error was encountered; no data loading was performed based on the arguments provided.");

                var universe =
                    universeLoader
                        .Perform(arguments);

                var result =
                    _domainRenderer
                        .Perform(universe);

                _fileWriter
                    .Perform(
                        outputPath,
                        result,
                        markdownAnchor
                    );

                _logger
                    .LogInformation("Siren operation completed");
            }
            catch (Exception ex)
            {
                _logger
                    .LogError($"Error encountered: {ex.Message}");

                return -2;
            }

            return 0;
        }
    }
}