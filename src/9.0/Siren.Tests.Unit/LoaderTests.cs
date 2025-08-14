using Microsoft.Extensions.Logging;
using NSubstitute;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.AssemblyLoad.Builders;
using Siren.Infrastructure.AssemblyLoad.Mapping;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Xunit;

namespace Siren.Tests.Unit
{
    public class LoaderTests
    {
        private readonly AssemblyLoader _sut;
        public LoaderTests()
        {
            var logger = Substitute.For<ILogger<AssemblyLoader>>();
            var entityBuilder = Substitute.For<IEntityBuilder>();
            var relationshipBuilder = Substitute.For<IRelationshipBuilder>();
            var assemblyMapper = Substitute.For<IAssemblyMapper>();
            _sut = new AssemblyLoader(logger, entityBuilder, relationshipBuilder, assemblyMapper);
        }

        [Fact]
        public void Test_FilterEntities_FiltersBySubstring()
        {
            // Arrange
            var entities = new[]
            {
                new ExtractedEntity { EntityName = "User" },
                new ExtractedEntity { EntityName = "Group" },
                new ExtractedEntity { EntityName = "UserGroup" },
                new ExtractedEntity { EntityName = "Role" },
                new ExtractedEntity { EntityName = "Permission" }
            };

            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = null
            };
            
            // Act
            var result = _sut.FilterEntities(entities, args);

            // Assert
            Assert.Contains(result, e => e.EntityName == "User");
            Assert.Contains(result, e => e.EntityName == "Group");
            Assert.Contains(result, e => e.EntityName == "UserGroup");
            Assert.DoesNotContain(result, e => e.EntityName == "Role");
            Assert.DoesNotContain(result, e => e.EntityName == "Permission");
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Test_FilterEntities_SkipEntities()
        {
            // Arrange
            var entities = new[]
            {
                new ExtractedEntity { EntityName = "User" },
                new ExtractedEntity { EntityName = "Group" },
                new ExtractedEntity { EntityName = "Role" }
            };
            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = "User,Role"
            };
            
            // Act
            var result = _sut.FilterEntities(entities, args);
            
            // Assert
            Assert.DoesNotContain(result, e => e.EntityName == "User");
            Assert.DoesNotContain(result, e => e.EntityName == "Role");
            Assert.Contains(result, e => e.EntityName == "Group");
            Assert.Single(result);
        }

        [Fact]
        public void Test_FilterEntities_FilterAndSkipCombined()
        {
            // Arrange
            var entities = new[]
            {
                new ExtractedEntity { EntityName = "User" },
                new ExtractedEntity { EntityName = "Group" },
                new ExtractedEntity { EntityName = "UserGroup" },
                new ExtractedEntity { EntityName = "Role" }
            };
            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = "UserGroup"
            };
            
            // Act
            var result = _sut.FilterEntities(entities, args);

            // Assert
            Assert.Contains(result, e => e.EntityName == "User");
            Assert.Contains(result, e => e.EntityName == "Group");
            Assert.DoesNotContain(result, e => e.EntityName == "UserGroup");
            Assert.DoesNotContain(result, e => e.EntityName == "Role");
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Test_FilterEntities_NoFiltersReturnsAll()
        {
            var entities = new[]
            {
                new ExtractedEntity { EntityName = "User" },
                new ExtractedEntity { EntityName = "Group" }
            };

            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = null
            };

            var result = _sut.FilterEntities(entities, args);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.EntityName == "User");
            Assert.Contains(result, e => e.EntityName == "Group");
        }
    }
}
