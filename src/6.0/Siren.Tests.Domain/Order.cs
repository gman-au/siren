using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siren.Tests.Domain
{
	public class Order
	{
		[Key]
		public Guid OrderId { get; set; }
		
		[ForeignKey("SomeOtherId")]
		public Guid CustomerId { get; set; }
		
		public long ReferenceNumber { get; set; }
		
		public DateTime DatePlaced { get; set; }
	}
}