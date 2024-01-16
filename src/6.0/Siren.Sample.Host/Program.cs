// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Reflection;
using Siren.Infrastructure.Mermaid;
using Siren.Infrastructure.Poco;
using Siren.Tests.Domain;


// var universe = new Universe { Entities = entities, Relationships = relationships};

var universe = await new AssemblyScanner().PerformAsync(Assembly.GetAssembly(typeof(Customer)));

var result = await new MermaidRenderer().RenderAsync(universe);

var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

File.WriteAllText(
	Path.Combine(
		path,
		"output.md"
	),
	result.ToString()
);