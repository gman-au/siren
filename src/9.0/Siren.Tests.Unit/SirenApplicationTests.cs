using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Siren.Application;
using Siren.Infrastructure.Io;
using Siren.Infrastructure.Rendering;
using Siren.Interfaces;
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
            private readonly SirenApplication _sut;
            private string[] _args;
            private int _exitCode;
            private readonly Func<string[], IProgramArguments> _argsFunc;

            public TestContext()
            {
                var universeFilter = Substitute.For<IUniverseFilter>();
                var universeLoader = Substitute.For<IUniverseLoader>();
                var domainRenderer = Substitute.For<IDomainRenderer>();
                var fileWriter = Substitute.For<IFileWriter>();
                var logger = Substitute.For<ILogger<SirenApplication>>();
                _argsFunc = Substitute.For<Func<string[], IProgramArguments>>();

                universeLoader.IsApplicable(null).ReturnsForAnyArgs(true);

                _sut =
                    new SirenApplication(
                        logger,
                        fileWriter,
                        domainRenderer,
                        [universeLoader],
                        universeFilter,
                        _argsFunc
                    );
            }

            public void ArrangeAssemblyPathArguments()
            {
                _argsFunc(null)
                    .ReturnsForAnyArgs(
                        new ProgramArguments
                        {
                            TestAssemblyPath = "C:\\test_path\\test_assembly.dll",
                            OutputFilePath = "C:\\test_path\\output_file.md",
                            MarkdownAnchor = null,
                            ConnectionString = null,
                            FilterEntities = null,
                            SkipEntities = null,
                            Errors = null
                        });
            }

            public void ArrangeMissingOutputArgument()
            {
                _argsFunc(null)
                    .ReturnsForAnyArgs(
                        new ProgramArguments
                        {
                            TestAssemblyPath = "C:\\test_path\\test_assembly.dll",
                            OutputFilePath = null,
                            MarkdownAnchor = "### HEADER",
                            ConnectionString = null,
                            FilterEntities = null,
                            SkipEntities = null,
                            Errors = ["Missing output file path"]
                        });
            }

            public void ArrangeConnectionStringArguments()
            {
                _argsFunc(null)
                    .ReturnsForAnyArgs(
                        new ProgramArguments
                        {
                            TestAssemblyPath = null,
                            OutputFilePath = "C:\\test_path\\output_file.md",
                            MarkdownAnchor = null,
                            ConnectionString = "db//my-connection-string",
                            FilterEntities = null,
                            SkipEntities = null,
                            Errors = null
                        });
            }

            public void ArrangeAllArguments()
            {
                _argsFunc(null)
                    .ReturnsForAnyArgs(
                        new ProgramArguments
                        {
                            TestAssemblyPath = "C:\\test_path\\test_assembly.dll",
                            OutputFilePath = "C:\\test_path\\output_file.md",
                            MarkdownAnchor = "### HEADER",
                            ConnectionString = "db//my-connection-string",
                            FilterEntities = null,
                            SkipEntities = null,
                            Errors = null
                        });
            }

            public void ActRunApplication()
            {
                _exitCode = _sut.Perform(_args);
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
