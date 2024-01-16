// See https://aka.ms/new-console-template for more information

using Siren.Domain;
using Siren.Infrastructure.Mermaid;

var entities = new List<Entity>();
var relationships = new List<Relationship>();

var customerEntity = new Entity
{
	Name = "CUSTOMER",
	Properties = new List<Property>
	{
		new() { Name = "CustomerId", Type = "Guid", IsPrimaryKey = true },
		new() { Name = "FirstName", Type = "string" },
		new() { Name = "LastName", Type = "string" },
	}
};

var orderEntity = new Entity
{
	Name = "ORDER",
	Properties = new List<Property>
	{
		new() { Name = "OrderId", Type = "Guid", IsPrimaryKey = true, IsUniqueKey = true },
		new() { Name = "CustomerId", Type = "Guid", IsForeignKey = true },
		new() { Name = "ReferenceNumber", Type = "long" },
		new() { Name = "DatePlaced", Type = "datetime" },
	}
};

entities.Add(customerEntity);
entities.Add(orderEntity);

relationships.Add(
	new Relationship
	{
		Source = customerEntity,
		Target = orderEntity,
		SourceCardinality = CardinalityTypeEnum.ExactlyOne,
		TargetCardinality = CardinalityTypeEnum.OneOrMore
	}
);

var universe = new Universe { Entities = entities, Relationships = relationships};

var result = await new MermaidRenderer().RenderAsync(universe);

var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

File.WriteAllText(
	Path.Combine(
		path,
		"output.md"
	),
	result.ToString()
);