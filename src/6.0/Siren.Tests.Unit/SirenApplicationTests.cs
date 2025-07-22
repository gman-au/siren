using Microsoft.Extensions.Logging;
using NSubstitute;
using Siren.Application;
using Siren.Infrastructure.AssemblyLoad;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Rendering;
using Xunit;

namespace Siren.Tests.Unit
{
    public class SirenApplicationTests
    {
        private readonly TestContext _context = new();
        
        [Fact]
        public void Test_Valid_Assembly_Arguments()
        {
            _context.ArrangeAssemblyPathArguments();
            _context.ActRunApplication();
            _context.AssertSuccessExitCode();
        }
        
        [Fact]
        public void Test_Valid_Connection_String_Arguments()
        {
            _context.ArrangeConnectionStringArguments();
            _context.ActRunApplication();
            _context.AssertSuccessExitCode();
        }
        
        [Fact]
        public void Test_Too_Many_Arguments()
        {
            _context.ArrangeAllArguments();
            _context.ActRunApplication();
            _context.AssertProgramErrorExitCode();
        }
        
        [Fact]
        public void Test_Missing_Output_Argument()
        {
            _context.ArrangeMissingOutputArgument();
            _context.ActRunApplication();
            _context.AssertArgumentErrorExitCode();
        }
        
        private class TestContext
        {
            private string[] _args; 
            private readonly SirenApplication _sut;
            private int _exitCode;

            public TestContext()
            {
                var assemblyLoader = Substitute.For<IAssemblyLoader>();
                var domainRenderer = Substitute.For<IDomainRenderer>();
                var fileWriter = Substitute.For<IFileWriter>();
                var logger = Substitute.For<ILogger<SirenApplication>>();

                _sut =
                    new SirenApplication(
                        logger,
                        fileWriter,
                        domainRenderer,
                        assemblyLoader
                    );
            }

            public void ArrangeAssemblyPathArguments()
            {
                _args = new []
                {
                    "-a",
                    "C:\\test_path\\test_assembly.dll",
                    "--outputPath",
                    "C:\\test_path\\output_file.md"
                };
            }

            public void ArrangeMissingOutputArgument()
            {
                _args = new []
                {
                    "-a",
                    "C:\\test_path\\test_assembly.dll",
                    "--markdownAnchor",
                    "### HEADER"
                };
            }

            public void ArrangeConnectionStringArguments()
            {
                _args = new []
                {
                    "-c",
                    "db//my-connection-string",
                    "--outputPath",
                    "C:\\test_path\\output_file.md"
                };
            }

            public void ArrangeAllArguments()
            {
                _args = new []
                {
                    "-a",
                    "C:\\test_path\\test_assembly.dll",
                    "-c",
                    "db//my-connection-string",
                    "--outputPath",
                    "C:\\test_path\\output_file.md",
                    "--markdownAnchor",
                    "### HEADER"
                };
            }

            public void ActRunApplication()
            {
                _exitCode = 
                    _sut
                        .Perform(_args);
            }

            public void AssertArgumentErrorExitCode()
            {
                Assert.Equal(-1, _exitCode);
            }

            public void AssertProgramErrorExitCode()
            {
                Assert.Equal(-2, _exitCode);
            }
            
            public void AssertSuccessExitCode()
            {
                Assert.Equal(0, _exitCode);
            }
        }
    }
}