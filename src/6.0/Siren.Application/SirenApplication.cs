using System;
using Microsoft.Extensions.Logging;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Parsing;
using Siren.Infrastructure.Rendering;

namespace Siren.Application
{
    public class SirenApplication : ISirenApplication
    {
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly IDomainRenderer _domainRenderer;
        private readonly IFileWriter _fileWriter;
        private readonly ILogger<SirenApplication> _logger;
        private readonly IProgramArgumentsParser _programArgumentsParser;

        public SirenApplication(
            ILogger<SirenApplication> logger,
            IProgramArgumentsParser programArgumentsParser,
            IFileWriter fileWriter,
            IDomainRenderer domainRenderer,
            IAssemblyLoader assemblyLoader
        )
        {
            _logger = logger;
            _programArgumentsParser = programArgumentsParser;
            _fileWriter = fileWriter;
            _domainRenderer = domainRenderer;
            _assemblyLoader = assemblyLoader;
        }

        public void Perform(string[] args)
        {
            try
            {
                _logger
                    .LogInformation("Starting Siren console...");

                var arguments =
                    _programArgumentsParser
                        .Parse(args);

                var outputPath = arguments.OutputFilePath;
                var markdownAnchor = arguments.MarkdownAnchor;

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
            }
        }
    }
}