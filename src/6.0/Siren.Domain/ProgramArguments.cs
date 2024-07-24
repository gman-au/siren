namespace Siren.Domain
{
    public class ProgramArguments
    {
        public string TestAssemblyPath { get; set; }
        
        public string OutputFilePath { get; set; }
        
        public string MarkdownAnchor { get; set; }

        public string DatabaseContext { get; set; }

        public override string ToString()
        {
            return
                $"TestAssemblyFolder: '{TestAssemblyPath}'\r\n" +
                $"OutputFilePath: '{OutputFilePath}'\r\n" +
                $"MarkdownAnchor: '{MarkdownAnchor}'\r\n" +
                $"DatabaseContext: '{DatabaseContext}'\r\n";
        }
    }
}