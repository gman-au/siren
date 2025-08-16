using System.Collections.Generic;
using CommandLine;

namespace Siren.Interfaces
{
    public interface IProgramArguments
    {
        public string TestAssemblyPath { get; set; }

        public string OutputFilePath { get; set; }

        public string MarkdownAnchor { get; set; }

        public string ConnectionString { get; set; }

        public string FilterEntities { get; set; }

        public string SkipEntities { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}