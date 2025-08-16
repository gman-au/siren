using System.Collections.Generic;
using CommandLine;

namespace Siren.Interfaces;

public interface IProgramArguments
{
    string TestAssemblyPath { get; }
    string OutputFilePath { get; }
    string MarkdownAnchor { get; }
    string ConnectionString { get; }
    string FilterEntities { get; }
    string SkipEntities { get; }
    IEnumerable<Error> Init(string[] args);
}