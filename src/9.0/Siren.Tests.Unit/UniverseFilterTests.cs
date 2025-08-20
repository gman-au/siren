using System.Linq;
using Siren.Application;
using Siren.Domain;
using Siren.Tool;
using Xunit;

namespace Siren.Tests.Unit
{
    public class UniverseFilterTests
    {
        private class TestContext
        {
            private ProgramArguments Arguments { get; set; }
            private Universe Universe { get; set; }
            public Universe Result { get; private set; }

            public void ArrangeEntities(params string[] entityNames)
            {
                Universe = new Universe
                {
                    Entities = entityNames.Select(n => new Entity { ShortName = n }).ToList()
                };
            }

            public void ArrangeEntitiesAndRelationships((string, string)[] relationships, params string[] entityNames)
            {
                var entities = entityNames.Select(n => new Entity { ShortName = n }).ToList();
                Universe = new Universe
                {
                    Entities = entities,
                    Relationships = relationships.Select(rel =>
                        new Relationship
                        {
                            Source = entities.First(e => e.ShortName == rel.Item1),
                            Target = entities.First(e => e.ShortName == rel.Item2)
                        }).ToList()
                };
            }

            public void ArrangeArguments(string filterEntities = null, string skipEntities = null)
            {
                Arguments = new ProgramArguments
                {
                    FilterEntities = filterEntities,
                    SkipEntities = skipEntities
                };
            }

            public void ActFilter()
            {
                var sut = new UniverseFilter(Arguments);
                Result = sut.FilterByEntity(Universe);
            }
        }

        [Fact]
        public void Test_FilterEntities_FiltersBySubstring()
        {
            var ctx = new TestContext();
            ctx.ArrangeArguments(filterEntities: "User,Group");
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
            ctx.ArrangeArguments(skipEntities: "User,Role");
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
            ctx.ArrangeArguments(filterEntities: "User,Group", skipEntities: "UserGroup");
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
            ctx.ArrangeArguments();
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
            ctx.ArrangeArguments(filterEntities: "User,Group");
            ctx.ArrangeEntitiesAndRelationships(
                [("User", "Group"), ("Group", "Role"), ("User", "Role")],
                "User", "Group", "Role"
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
            ctx.ArrangeArguments(skipEntities: "Role");
            ctx.ArrangeEntitiesAndRelationships(
                [("User", "Group"), ("Group", "Role"), ("User", "Role")],
                "User", "Group", "Role"
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
            ctx.ArrangeArguments(filterEntities: "NonExistent");
            ctx.ArrangeEntitiesAndRelationships(
                [("User", "Group")],
                "User", "Group"
            );
            ctx.ActFilter();

            Assert.Empty(ctx.Result.Entities);
            Assert.Empty(ctx.Result.Relationships);
        }
    }
}