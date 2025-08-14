using Microsoft.Extensions.Logging;
using NSubstitute;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.AssemblyLoad.Builders;
using Siren.Infrastructure.AssemblyLoad.Mapping;
using Siren.Infrastructure.AssemblyLoad.Domain;
using Siren.Infrastructure.SchemaSearch;
using Xunit;

namespace Siren.Tests.Unit
{
    public class LoaderTests
    {
        private readonly AssemblyLoader _assemblyLoader;
        private readonly ConnectionStringLoader _connectionStringLoader;

        public LoaderTests()
        {
            var logger = Substitute.For<ILogger<AssemblyLoader>>();
            var entityBuilder = Substitute.For<IEntityBuilder>();
            var relationshipBuilder = Substitute.For<IRelationshipBuilder>();
            var assemblyMapper = Substitute.For<IAssemblyMapper>();
            var searchApplication = Substitute.For<ISearchApplication>();
            _assemblyLoader = new AssemblyLoader(logger, entityBuilder, relationshipBuilder, assemblyMapper);
            _connectionStringLoader = new ConnectionStringLoader(searchApplication);
        }

        [Fact]
        public void Test_AssemblyFilterEntities_FiltersBySubstring()
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
            var result = _assemblyLoader.FilterEntities(entities, args);

            // Assert
            Assert.Contains(result, e => e.EntityName == "User");
            Assert.Contains(result, e => e.EntityName == "Group");
            Assert.Contains(result, e => e.EntityName == "UserGroup");
            Assert.DoesNotContain(result, e => e.EntityName == "Role");
            Assert.DoesNotContain(result, e => e.EntityName == "Permission");
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Test_AssemblyFilterEntities_SkipEntities()
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
            var result = _assemblyLoader.FilterEntities(entities, args);
            
            // Assert
            Assert.DoesNotContain(result, e => e.EntityName == "User");
            Assert.DoesNotContain(result, e => e.EntityName == "Role");
            Assert.Contains(result, e => e.EntityName == "Group");
            Assert.Single(result);
        }

        [Fact]
        public void Test_AssemblyFilterEntities_FilterAndSkipCombined()
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
            var result = _assemblyLoader.FilterEntities(entities, args);

            // Assert
            Assert.Contains(result, e => e.EntityName == "User");
            Assert.Contains(result, e => e.EntityName == "Group");
            Assert.DoesNotContain(result, e => e.EntityName == "UserGroup");
            Assert.DoesNotContain(result, e => e.EntityName == "Role");
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Test_AssemblyFilterEntities_NoFiltersReturnsAll()
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

            var result = _assemblyLoader.FilterEntities(entities, args);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.EntityName == "User");
            Assert.Contains(result, e => e.EntityName == "Group");
        }
        
        [Fact]
        public void Test_ConnectionStringFilterEntities_FiltersBySubstring()
        {
            // Arrange
            var entities = new[]
            {
                new Entity { ShortName = "User" },
                new Entity { ShortName = "Group" },
                new Entity { ShortName = "UserGroup" },
                new Entity { ShortName = "Role" },
                new Entity { ShortName = "Permission" }
            };

            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = null
            };
            
            // Act
            var result = _connectionStringLoader.FilterEntities(entities, args);

            // Assert
            Assert.Contains(result, e => e.ShortName == "User");
            Assert.Contains(result, e => e.ShortName == "Group");
            Assert.Contains(result, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(result, e => e.ShortName == "Role");
            Assert.DoesNotContain(result, e => e.ShortName == "Permission");
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Test_ConnectionStringFilterEntities_SkipEntities()
        {
            // Arrange
            var entities = new[]
            {
                new Entity { ShortName = "User" },
                new Entity { ShortName = "Group" },
                new Entity { ShortName = "Role" }
            };
            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = "User,Role"
            };
            
            // Act
            var result = _connectionStringLoader.FilterEntities(entities, args);
            
            // Assert
            Assert.DoesNotContain(result, e => e.ShortName == "User");
            Assert.DoesNotContain(result, e => e.ShortName == "Role");
            Assert.Contains(result, e => e.ShortName == "Group");
            Assert.Single(result);
        }

        [Fact]
        public void Test_ConnectionStringFilterEntities_FilterAndSkipCombined()
        {
            // Arrange
            var entities = new[]
            {
                new Entity { ShortName = "User" },
                new Entity { ShortName = "Group" },
                new Entity { ShortName = "UserGroup" },
                new Entity { ShortName = "Role" }
            };
            var args = new ProgramArguments
            {
                FilterEntities = "User,Group",
                SkipEntities = "UserGroup"
            };
            
            // Act
            var result = _connectionStringLoader.FilterEntities(entities, args);

            // Assert
            Assert.Contains(result, e => e.ShortName == "User");
            Assert.Contains(result, e => e.ShortName == "Group");
            Assert.DoesNotContain(result, e => e.ShortName == "UserGroup");
            Assert.DoesNotContain(result, e => e.ShortName == "Role");
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Test_ConnectionStringFilterEntities_NoFiltersReturnsAll()
        {
            var entities = new[]
            {
                new Entity { ShortName = "User" },
                new Entity { ShortName = "Group" }
            };

            var args = new ProgramArguments
            {
                FilterEntities = null,
                SkipEntities = null
            };

            var result = _connectionStringLoader.FilterEntities(entities, args);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.ShortName == "User");
            Assert.Contains(result, e => e.ShortName == "Group");
        }
    }
}
