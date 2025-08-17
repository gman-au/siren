using System.Collections.Generic;

namespace Siren.Interfaces;

public interface IProgramArguments
{
    string TestAssemblyPath { get; }
    string OutputFilePath { get; }
    string MarkdownAnchor { get; }
    string MermaidBlockStyle { get; }
    string MermaidThemeLine { get; }
    string ConnectionString { get; }
    string FilterEntities { get; }
    string SkipEntities { get; }
    IEnumerable<IArgumentError> Initialize(string[] args);
}