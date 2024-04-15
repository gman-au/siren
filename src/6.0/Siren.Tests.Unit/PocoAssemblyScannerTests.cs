// using System.Linq;
// using System.Reflection;
// using Siren.Domain;
// using Siren.Infrastructure.AssemblyLoad;
// using Siren.Tests.Domain;
// using Xunit;
//
// namespace Siren.Tests.Unit
// {
//     public class PocoAssemblyScannerTests
//     {
//         private readonly TestContext _testContext = new();
//
//         [Fact]
//         public void Test_Scanning_Of_Domain_Assembly()
//         {
//             _testContext.ArrangeAssembly();
//             _testContext.ActScanAssembly();
//             _testContext.AssertResult();
//         }
//
//         private class TestContext
//         {
//             private Assembly _assembly;
//             private Universe _result;
//
//             public void ArrangeAssembly()
//             {
//                 _assembly =
//                     Assembly
//                         .GetAssembly(typeof(Customer));
//             }
//
//             public void ActScanAssembly()
//             {
//                 _result =
//                     PocoAssemblyScanner
//                         .Perform(_assembly);
//             }
//
//             public void AssertResult()
//             {
//                 var entities = _result.Entities.ToArray();
//                 var relationships = _result.Relationships;
//
//                 Assert.Equal(
//                     2,
//                     entities.Count()
//                 );
//                 Assert.Single(relationships);
//
//                 Assert.Equal(
//                     "Customer",
//                     entities[0].ShortName
//                 );
//                 Assert.Equal(
//                     4,
//                     entities[0].Properties.Count()
//                 );
//
//                 Assert.Equal(
//                     "Order",
//                     entities[1].ShortName
//                 );
//                 Assert.Equal(
//                     4,
//                     entities[1].Properties.Count()
//                 );
//             }
//         }
//     }
// }