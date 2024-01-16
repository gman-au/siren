using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Siren.Domain;
using Siren.Interfaces;

namespace Siren.Infrastructure.Poco
{
	public class AssemblyScanner : IAssemblyScanner
	{
		public async Task<Universe> PerformAsync(Assembly assembly)
		{
			var entities = new List<Entity>();
			
			var pocos =
				assembly
					.GetExportedTypes()
					.Where(
						o =>
							o.IsClass
					);

			foreach (var poco in pocos)
			{
				var name = poco.Name;
				var properties = new List<Property>();
				
				foreach (var declaredProperty in poco.GetProperties())
				{
					var property = new Property
					{
						Name = declaredProperty.Name,
						Type = declaredProperty.PropertyType.Name,
						IsPrimaryKey = ContainsAttribute<KeyAttribute>(declaredProperty),
						IsForeignKey = ContainsAttribute<ForeignKeyAttribute>(declaredProperty),
						IsUniqueKey = false
					};

					properties.Add(property);
				}

				var entity =
					new Entity
					{
						Name = name,
						Properties = properties
					};
				
				entities.Add(entity);
			}
			
			var relationships = new List<Relationship>();

			// var customerEntity = new Entity
			// {
			// 	Name = "CUSTOMER",
			// 	Properties = new List<Property>
			// 	{
			// 		new() { Name = "CustomerId", Type = "Guid", IsPrimaryKey = true },
			// 		new() { Name = "FirstName", Type = "string" },
			// 		new() { Name = "LastName", Type = "string" },
			// 	}
			// };
			//
			// var orderEntity = new Entity
			// {
			// 	Name = "ORDER",
			// 	Properties = new List<Property>
			// 	{
			// 		new() { Name = "OrderId", Type = "Guid", IsPrimaryKey = true, IsUniqueKey = true },
			// 		new() { Name = "CustomerId", Type = "Guid", IsForeignKey = true },
			// 		new() { Name = "ReferenceNumber", Type = "long" },
			// 		new() { Name = "DatePlaced", Type = "datetime" },
			// 	}
			// };
			//
			// entities.Add(customerEntity);
			// entities.Add(orderEntity);
			//
			// relationships.Add(
			// 	new Relationship
			// 	{
			// 		Source = customerEntity,
			// 		Target = orderEntity,
			// 		SourceCardinality = CardinalityTypeEnum.ExactlyOne,
			// 		TargetCardinality = CardinalityTypeEnum.OneOrMore
			// 	}
			// );

			var result = new Universe { Entities = entities, Relationships = relationships };
			return result;
		}

		private static bool ContainsAttribute<T>(MemberInfo property) where T : Attribute
		{
			var customAttributes = 
				property
					.CustomAttributes;

			var exists =
				customAttributes
					.Any(o => o.AttributeType == typeof(T));
			
			return exists;
		}
	}
}