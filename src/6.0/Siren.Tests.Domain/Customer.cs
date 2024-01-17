using System;
using System.ComponentModel.DataAnnotations;

namespace Siren.Tests.Domain
{
	public class Customer
	{
		[Key]
		public Guid CustomerId { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }
	}
}