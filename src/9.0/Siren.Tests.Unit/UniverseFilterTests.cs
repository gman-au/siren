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
            // Arrange
            var programArguments = new ProgramArguments { FilterEntities = "User,Group" };
            var sut = new UniverseFilter(programArguments);
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
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
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
            // Arrange
            var programArguments = new ProgramArguments { SkipEntities = "User,Role" };
            var sut = new UniverseFilter(programArguments);
            var universe = new Universe
            {
                Entities =
                [
                    new Entity { ShortName = "User" },
                    new Entity { ShortName = "Group" },
                    new Entity { ShortName = "Role" }
                ]
            };
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "User");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "Role");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
            Assert.Single(result.Entities);
        }

        [Fact]
        public void Test_FilterEntities_FilterAndSkipCombined()
        {
            // Arrange
            var programArguments = new ProgramArguments { FilterEntities = "User,Group", SkipEntities = "UserGroup" };
            var sut = new UniverseFilter(programArguments);
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
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
            Assert.Contains(result.Entities, e => e.ShortName == "User");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(result.Entities, e => e.ShortName == "Role");
            Assert.Equal(2, result.Entities.Count());
        }

        [Fact]
        public void Test_FilterEntities_NoFiltersReturnsAll()
        {
            // Arrange
            var programArguments = new ProgramArguments();
            var sut = new UniverseFilter(programArguments);
            var universe = new Universe
            {
                Entities =
                [
                    new Entity { ShortName = "User" },
                    new Entity { ShortName = "Group" }
                ]
            };
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
            Assert.Equal(2, result.Entities.Count());
            Assert.Contains(result.Entities, e => e.ShortName == "User");
            Assert.Contains(result.Entities, e => e.ShortName == "Group");
        }

        [Fact]
        public void Test_FilterEntities_FiltersRelationshipsByEntities()
        {
            // Arrange
            var programArguments = new ProgramArguments { FilterEntities = "User,Group" };
            var sut = new UniverseFilter(programArguments);
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
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
            Assert.Contains(result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Group");
            Assert.DoesNotContain(result.Relationships,
                r => r.Source.ShortName == "group" && r.Target.ShortName == "Role");
            Assert.DoesNotContain(result.Relationships,
                r => r.Source.ShortName == "User" && r.Target.ShortName == "Role");
            Assert.Single(result.Relationships);
        }

        [Fact]
        public void Test_FilterEntities_RemovesRelationshipsWithSkippedEntities()
        {
            // Arrange
            var programArguments = new ProgramArguments { SkipEntities = "Role" };
            var sut = new UniverseFilter(programArguments);
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
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
            Assert.Contains(result.Relationships, r => r.Source.ShortName == "User" && r.Target.ShortName == "Group");
            Assert.DoesNotContain(result.Relationships,
                r => r.Source.ShortName == "Group" && r.Target.ShortName == "Role");
            Assert.DoesNotContain(result.Relationships,
                r => r.Source.ShortName == "User" && r.Target.ShortName == "Role");
            Assert.Single(result.Relationships);
        }

        [Fact]
        public void Test_FilterEntities_NoEntitiesMeansNoRelationships()
        {
            // Arrange
            var programArguments = new ProgramArguments { FilterEntities = "NonExistent" };
            var sut = new UniverseFilter(programArguments);
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
            // Act
            var result = sut.FilterEntities(universe);
            // Assert
            Assert.Empty(result.Entities);
            Assert.Empty(result.Relationships);
        }
    }
}