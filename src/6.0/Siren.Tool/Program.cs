// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Reflection;
using Siren.Infrastructure.Mermaid;
using Siren.Infrastructure.Poco;

Console.WriteLine("Hello, World!");

if (args.Length < 2)
	throw new Exception("Expected 2 arguments");

var assemblyPath = args[0];
var outputPath = args[1];

if (string.IsNullOrEmpty(assemblyPath))
	throw new Exception("Assembly path argument invalid");

if (string.IsNullOrEmpty(outputPath))
	throw new Exception("Output path argument invalid");

Assembly assembly;

try
{
	assembly =
		Assembly
			.LoadFile(assemblyPath);
}
catch (FileNotFoundException)
{
	throw new Exception($"Could not load assembly from {assemblyPath}");
}

var universe =
	await
		new AssemblyScanner()
			.PerformAsync(assembly);

var result =
	await
		new MermaidRenderer()
			.RenderAsync(universe);

var path =
	Environment
		.GetFolderPath(Environment.SpecialFolder.MyDocuments);

File
	.WriteAllText(
		outputPath,
		result
			.ToString()
	);