using System;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Logging;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Rendering;

namespace Siren.Application
{
    public class SirenApplication : ISirenApplication
    {
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly IDomainRenderer _domainRenderer;
        private readonly IFileWriter _fileWriter;
        private readonly ILogger<SirenApplication> _logger;

        public SirenApplication(
            ILogger<SirenApplication> logger,
            IFileWriter fileWriter,
            IDomainRenderer domainRenderer,
            IAssemblyLoader assemblyLoader
        )
        {
            _logger = logger;
            _fileWriter = fileWriter;
            _domainRenderer = domainRenderer;
            _assemblyLoader = assemblyLoader;
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
                
                if (!string.IsNullOrWhiteSpace(arguments.TestAssemblyPath) && !string.IsNullOrWhiteSpace(arguments.ConnectionString))
                    throw new Exception("Specify one of either test assembly path or connection string.");

                var universe =
                    _assemblyLoader
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