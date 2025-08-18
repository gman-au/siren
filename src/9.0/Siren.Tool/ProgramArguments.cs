using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Siren.Interfaces;

namespace Siren.Tool
{
    public class ProgramArguments : IProgramArguments
    {
        [Option('a', "assemblyPath", Required = false, HelpText = "Migration assembly file path.")]
        public string TestAssemblyPath { get; set; }

        [Option('o', "outputPath", Required = true, HelpText = "Output markdown file path.")]
        public string OutputFilePath { get; set; }

        [Option('m', "markdownAnchor", Required = false, HelpText = "Markdown anchor for defined section.")]
        public string MarkdownAnchor { get; set; }

        [Option('c', "connectionString", Required = false, HelpText = "Database connection string.")]
        public string ConnectionString { get; set; }

        [Option('f', "filterEntities", Required = false, HelpText =
            "Comma-separated list of entity-substrings to filter. " +
            "Only entities that contain any of these substrings will be included in the output. " +
            "E.g. 'User,Group' will select all entities that contain 'User' or 'Group' in name.")]
        public string FilterEntities { get; set; }

        [Option('s', "skipEntities", Required = false, HelpText = "Comma-separated list of entities to skip.")]
        public string SkipEntities { get; set; }

        [Option('t', "template", Required = false, HelpText = "Render template (e.g. AzureDevOps). Default is 'default'.", Default = "default")]
        public string RenderTemplate { get; set; }

        public IEnumerable<IArgumentError> Initialize(string[] args)
        {
            var parsedArguments = Parser.Default.ParseArguments<ProgramArguments>(args);
            if (parsedArguments.Tag == ParserResultType.Parsed)
            {
                var arguments = parsedArguments.Value;
                TestAssemblyPath = arguments.TestAssemblyPath;
                OutputFilePath = arguments.OutputFilePath;
                MarkdownAnchor = arguments.MarkdownAnchor;
                ConnectionString = arguments.ConnectionString;
                FilterEntities = arguments.FilterEntities;
                SkipEntities = arguments.SkipEntities;
                RenderTemplate = arguments.RenderTemplate;
            }

            return parsedArguments.Errors.Select(error =>
                new ArgumentError(error.ToString())).Cast<IArgumentError>().ToList();
        }

        public override string ToString()
        {
            return $"TestAssemblyFolder: '{TestAssemblyPath}'\r\n"
                   + $"OutputFilePath: '{OutputFilePath}'\r\n"
                   + $"MarkdownAnchor: '{MarkdownAnchor}'\r\n"
                   + $"ConnectionString: '{ConnectionString}'\r\n"
                   + $"FilterEntities: '{FilterEntities}'\r\n"
                   + $"SkipEntities: '{SkipEntities}'\r\n"
                   + $"RenderTemplate: '{RenderTemplate}'\r\n";
        }
    }

    public class ArgumentError : IArgumentError
    {
        public string Message { get; }

        public ArgumentError(string message)
        {
            Message = message;
        }
    }
}