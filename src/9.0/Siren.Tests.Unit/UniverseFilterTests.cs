using System.Linq;
using Siren.Application;
using Siren.Domain;
using Siren.Interfaces;
using Xunit;

namespace Siren.Tests.Unit
{
    public class UniverseFilterTests
    {
        private readonly IUniverseFilter _sut = new UniverseFilter();
        
        [Fact]
        public void Test_FilterEntities_FiltersBySubstring()
        {
            var universe = new Universe
            {
                Entities =
                [
                    new Entity { ShortName = "User" },
                    new Entity { ShortName = "Group" },
                    new Entity { ShortName = "UserGroup" },
                    new Entity { ShortName = "Role" },
                    new Entity { ShortName = "Permission" }
                ]
            };

            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = null
            };

            var result = _sut.FilterEntities(universe, args);

            Assert.Contains(result.Entities, e => e.ShortName == "User");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
            Assert.Contains(result.Entities, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "Role");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "Permission");
            Assert.Equal(3, result.Entities.Count());
        }

        [Fact]
        public void Test_FilterEntities_SkipEntities()
        {
            var universe = new Universe
            {
                Entities =
                [
                    new Entity { ShortName = "User" },
                    new Entity { ShortName = "Group" },
                    new Entity { ShortName = "Role" }
                ]
            };
            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = "User,Role"
            };

            var result = _sut.FilterEntities(universe, args);

            Assert.DoesNotContain(result.Entities, e => e.ShortName == "User");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "Role");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
            Assert.Single(result.Entities);
        }

        [Fact]
        public void Test_FilterEntities_FilterAndSkipCombined()
        {
            var universe = new Universe
            {
                Entities =
                [
                    new Entity { ShortName = "User" },
                    new Entity { ShortName = "Group" },
                    new Entity { ShortName = "UserGroup" },
                    new Entity { ShortName = "Role" }
                ]
            };
            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = "UserGroup"
            };

            var result = _sut.FilterEntities(universe, args);

            Assert.Contains(result.Entities, e => e.ShortName == "User");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "Role");
            Assert.Equal(2, result.Entities.Count());
        }

        [Fact]
        public void Test_FilterEntities_NoFiltersReturnsAll()
        {
            var universe = new Universe
            {
                Entities =
                [
                    new Entity { ShortName = "User" },
                    new Entity { ShortName = "Group" }
                ]
            };

            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = null
            };

            var result = _sut.FilterEntities(universe, args);

            Assert.Equal(2, result.Entities.Count());
            Assert.Contains(result.Entities, e => e.ShortName == "User");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
        }

        [Fact]
        public void Test_FilterEntities_FiltersRelationshipsByEntities()
        {
            var user = new Entity { ShortName = "User" };
            var group = new Entity { ShortName = "Group" };
            var role = new Entity { ShortName = "Role" };

            var universe = new Universe
            {
                Entities = [user, group, role],
                Relationships =
                [
                    new Relationship { Source = user, Target = group },
                    new Relationship { Source = group, Target = role },
                    new Relationship { Source = user, Target = role }
                ]
            };

            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = null
            };

            var result = _sut.FilterEntities(universe, args);

            // Only relationships where both Source and Target are in filtered entities
            Assert.Contains(result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Group");
            Assert.DoesNotContain(result.Relationships, r => r.Source.ShortName == "group" && r.Target.ShortName == "Role");
            Assert.DoesNotContain(result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Role");
            Assert.Single(result.Relationships);
        }

        [Fact]
        public void Test_FilterEntities_RemovesRelationshipsWithSkippedEntities()
        {
            var user = new Entity { ShortName = "User" };
            var group = new Entity { ShortName = "Group" };
            var role = new Entity { ShortName = "Role" };

            var universe = new Universe
            {
                Entities = [user, group, role],
                Relationships =
                [
                    new Relationship { Source = user, Target = group },
                    new Relationship { Source = group, Target = role },
                    new Relationship { Source = user, Target = role }
                ]
            };

            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = "Role"
            };

            var result = _sut.FilterEntities(universe, args);

            // Relationships with Role as Source or Target should be removed
            Assert.Contains(result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Group");
            Assert.DoesNotContain(result.Relationships, r => r.Source.ShortName == "Group" && r.Target.ShortName == "Role");
            Assert.DoesNotContain(result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Role");
            Assert.Single(result.Relationships);
        }

        [Fact]
        public void Test_FilterEntities_NoEntitiesMeansNoRelationships()
        {
            var user = new Entity { ShortName = "User" };
            var group = new Entity { ShortName = "Group" };

            var universe = new Universe
            {
                Entities = [user, group],
                Relationships =
                [
                    new Relationship { Source = user, Target = group }
                ]
            };

            var args = new ProgramArguments
            {
                FilterEntities = "NonExistent",
                SkipEntities = null
            };

            var result = _sut.FilterEntities(universe, args);

            Assert.Empty(result.Entities);
            Assert.Empty(result.Relationships);
        }
    }
}