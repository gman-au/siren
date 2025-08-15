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

        [Option('b', "mermaidAnchorBegin", Required = false, HelpText = "Mermaid beginning, instead of: ```mermaid")]
        public string MermaidAnchorStart { get; set; }

        [Option('e', "mermaidAnchorEnd", Required = false, HelpText = "Markdown ending, instead of: ```")]
        public string MermaidAnchorEnd { get; set; }
        
        [Option('t', "mermaidThemeLine", Required = false, HelpText = "Theme line for Mermaid diagram.")]
        public string MermaidThemeLine { get; set; }

        [Option('c', "connectionString", Required = false, HelpText = "Database connection string.")]
        public string ConnectionString { get; set; }

        public override string ToString()
        {
            return $"TestAssemblyFolder: '{TestAssemblyPath}'\r\n"
                + $"OutputFilePath: '{OutputFilePath}'\r\n"
                + $"MarkdownAnchor: '{MarkdownAnchor}'\r\n"
                + $"MermaidAnchorStart: '{MermaidAnchorStart}'\r\n"
                + $"MermaidAnchorEnd: '{MermaidAnchorEnd}'\r\n"
                + $"MermaidThemeLine: '{MermaidThemeLine}'\r\n"
                + $"ConnectionString: '{ConnectionString}'\r\n";
        }
    }
}
