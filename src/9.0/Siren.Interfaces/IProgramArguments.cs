using System.Collections.Generic;

namespace Siren.Interfaces;

public interface IProgramArguments
{
    string TestAssemblyPath { get; }
    string OutputFilePath { get; }
    string MarkdownAnchor { get; }
    bool WrapUsingColons { get; }
    string ConnectionString { get; }
    string FilterEntities { get; }
    string SkipEntities { get; }
    IEnumerable<IArgumentError> Initialize(string[] args);
}