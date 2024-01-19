using System;
using System.IO;
using System.Reflection;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Mermaid;

Console
    .WriteLine("Starting Siren console...");

if (args.Length < 2)
    throw new Exception("Expected 2 arguments");

var assemblyPath = args[0];
var outputPath = args[1];
var markdownAnchor = (string)null;

if (args.Length > 2)
{
    markdownAnchor = args[2];
}

if (string.IsNullOrEmpty(assemblyPath))
    throw new Exception("Assembly path argument invalid");

if (string.IsNullOrEmpty(outputPath))
    throw new Exception("Output path argument invalid");

Assembly assembly;
var isASnapshotAssembly = false;

try
{
    assembly =
        Assembly
            .LoadFrom(assemblyPath);

    isASnapshotAssembly = 
        assembly
            .IsASnapshotAssembly();
}
catch (FileNotFoundException)
{
    throw new Exception($"Could not load assembly from {assemblyPath}");
}

Universe universe;

if (isASnapshotAssembly) {
    Console
        .WriteLine("ModelSnapshot type found in assembly... scanning...");
    
    universe =
        SnapshotAssemblyScanner
            .Perform(assembly);
}
else
{
    Console
        .WriteLine("POCO default assembly... scanning...");
    
    universe =
        PocoAssemblyScanner
            .Perform(assembly);
}

var result =
    MermaidRenderer
        .Perform(universe);

FileWriter
    .Perform(
        outputPath,
        result,
        markdownAnchor
    );

Console
    .WriteLine("Siren operation completed");