using System.Collections.Generic;

namespace Siren.Interfaces
{
    public interface IProgramArguments
    {
        string TestAssemblyPath { get; }
        string OutputFilePath { get; }
        string MarkdownAnchor { get; }
        string ConnectionString { get; }
        string FilterEntities { get; }
        string SkipEntities { get; }
        string RenderTemplate { get; }
        string FilterSchemas { get; }
        string SkipSchemas { get; }
        string CustomLayoutHeader { get; }
        IEnumerable<IArgumentError> Initialize(string[] args);
    }
}