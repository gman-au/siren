using System.Linq;
using Siren.Application;
using Siren.Domain;
using Siren.Tool;
using Xunit;

namespace Siren.Tests.Unit
{
    public class UniverseFilterTests
    {
        [Fact]
        public void Test_FilterEntities_FiltersBySubstring()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments("User,Group");
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntities("User", "Group", "UserGroup", "Role", "Permission");
            ctx.ActFilter();

            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "User");
            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "Group");
            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(ctx.Result.Entities, e => e.ShortName == "Role");
            Assert.DoesNotContain(ctx.Result.Entities, e => e.ShortName == "Permission");
            Assert.Equal(3, ctx.Result.Entities.Count());
        }

        [Fact]
        public void Test_FilterEntities_SkipEntities()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments(skipEntities: "User,Role");
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntities("User", "Group", "Role");
            ctx.ActFilter();

            Assert.DoesNotContain(ctx.Result.Entities, e => e.ShortName == "User");
            Assert.DoesNotContain(ctx.Result.Entities, e => e.ShortName == "Role");
            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "Group");
            Assert.Single(ctx.Result.Entities);
        }

        [Fact]
        public void Test_FilterEntities_FilterAndSkipCombined()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments("User,Group", "UserGroup");
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntities("User", "Group", "UserGroup", "Role");
            ctx.ActFilter();

            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "User");
            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "Group");
            Assert.DoesNotContain(ctx.Result.Entities, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(ctx.Result.Entities, e => e.ShortName == "Role");
            Assert.Equal(2, ctx.Result.Entities.Count());
        }

        [Fact]
        public void Test_FilterEntities_NoFiltersReturnsAll()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments();
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntities("User", "Group");
            ctx.ActFilter();

            Assert.Equal(2, ctx.Result.Entities.Count());
            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "User");
            Assert.Contains(ctx.Result.Entities, e => e.ShortName == "Group");
        }

        [Fact]
        public void Test_FilterEntities_FiltersRelationshipsByEntities()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments("User,Group");
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntitiesAndRelationships(
                [("User", "Group"), ("Group", "Role"), ("User", "Role")],
                ("dbo", "User"),
                ("dbo", "Group"),
                ("dbo", "Role")
            );
            ctx.ActFilter();

            Assert.Contains(ctx.Result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Group");
            Assert.DoesNotContain(ctx.Result.Relationships, r => r.Source.ShortName == "Group" && r.Target.ShortName == "Role");
            Assert.DoesNotContain(ctx.Result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Role");
            Assert.Single(ctx.Result.Relationships);
        }

        [Fact]
        public void Test_FilterEntities_RemovesRelationshipsWithSkippedEntities()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments(skipEntities: "Role");
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntitiesAndRelationships(
                [("User", "Group"), ("Group", "Role"), ("User", "Role")],
                ("dbo", "User"),
                ("dbo", "Group"),
                ("dbo", "Role")
            );
            ctx.ActFilter();

            Assert.Contains(ctx.Result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Group");
            Assert.DoesNotContain(ctx.Result.Relationships, r => r.Source.ShortName == "Group" && r.Target.ShortName == "Role");
            Assert.DoesNotContain(ctx.Result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Role");
            Assert.Single(ctx.Result.Relationships);
        }

        [Fact]
        public void Test_FilterEntities_NoEntitiesMeansNoRelationships()
        {
            var ctx = new TestContext();
            ctx.ArrangeEntityFilterArguments("NonExistent");
            ctx.ArrangeSchemaFilterArguments();
            ctx.ArrangeEntitiesAndRelationships(
                [("User", "Group")],
                ("dbo", "User"),
                ("dbo", "Group")
            );
            ctx.ActFilter();

            Assert.Empty(ctx.Result.Entities);
            Assert.Empty(ctx.Result.Relationships);
        }

        private class TestContext
        {
            private ProgramArguments Arguments { get; set; }
            private Universe Universe { get; set; }
            public Universe Result { get; private set; }

            public void ArrangeEntities(params string[] entityNames)
            {
                Universe = new Universe
                {
                    Entities =
                        entityNames
                            .Select(n => new Entity { ShortName = n })
                            .ToList()
                };
            }

            public void ArrangeEntitiesAndRelationships((string, string)[] relationships, params (string, string)[] entityNames)
            {
                var entities =
                    entityNames
                        .Select(n => new Entity { Schema = n.Item1, ShortName = n.Item2 })
                        .ToList();

                Universe = new Universe
                {
                    Entities = entities,
                    Relationships =
                        relationships
                            .Select(rel =>
                                new Relationship
                                {
                                    Source = entities.First(e => e.ShortName == rel.Item1),
                                    Target = entities.First(e => e.ShortName == rel.Item2)
                                })
                            .ToList()
                };
            }

            public void ArrangeEntityFilterArguments(string filterEntities = null, string skipEntities = null)
            {
                Arguments = Arguments ?? new ProgramArguments();
                Arguments.FilterEntities = filterEntities;
                Arguments.SkipEntities = skipEntities;
            }

            public void ArrangeSchemaFilterArguments(string filterSchemas = null, string skipSchemas = null)
            {
                Arguments = Arguments ?? new ProgramArguments();
                Arguments.FilterSchemas = filterSchemas;
                Arguments.SkipSchemas = skipSchemas;
            }

            public void ActFilter()
            {
                var sut = new UniverseFilter(Arguments);
                Result = sut.FilterByEntity(Universe);
            }
        }
    }
}