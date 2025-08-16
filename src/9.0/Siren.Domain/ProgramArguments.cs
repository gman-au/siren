using CommandLine;

namespace Siren.Domain
{
    public class ProgramArguments
    {
        [Option('a', "assemblyPath", Required = false, HelpText = "Migration assembly file path.")]
        public string TestAssemblyPath { get; set; }

        [Option('o', "outputPath", Required = true, HelpText = "Output markdown file path.")]
        public string OutputFilePath { get; set; }

        [Option('m', "markdownAnchor", Required = false, HelpText = "Markdown anchor for defined section.")]
        public string MarkdownAnchor { get; set; }

        [Option('b', "mermaidBlockBegin", Required = false, HelpText = "Mermaid beginning, instead of: ```mermaid")]
        public string MermaidBlockBegin { get; set; }

        [Option('e', "mermaidBlockEnd", Required = false, HelpText = "Markdown ending, instead of: ```")]
        public string MermaidBlockEnd { get; set; }
        
        [Option('t', "mermaidThemeLine", Required = false, HelpText = "Theme line for Mermaid diagram.")]
        public string MermaidThemeLine { get; set; }

        [Option('c', "connectionString", Required = false, HelpText = "Database connection string.")]
        public string ConnectionString { get; set; }

        public override string ToString()
        {
            return $"TestAssemblyFolder: '{TestAssemblyPath}'\r\n"
                + $"OutputFilePath: '{OutputFilePath}'\r\n"
                + $"MarkdownAnchor: '{MarkdownAnchor}'\r\n"
                + $"MermaidBlockBegin: '{MermaidBlockBegin}'\r\n"
                + $"MermaidBlockEnd: '{MermaidBlockEnd}'\r\n"
                + $"MermaidThemeLine: '{MermaidThemeLine}'\r\n"
                + $"ConnectionString: '{ConnectionString}'\r\n";
        }
    }
}
