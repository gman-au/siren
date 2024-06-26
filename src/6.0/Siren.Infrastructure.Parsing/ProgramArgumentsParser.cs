﻿using System;
using Microsoft.Extensions.Logging;
using Siren.Domain;

namespace Siren.Infrastructure.Parsing
{
    public class ProgramArgumentsParser : IProgramArgumentsParser
    {
        private readonly ILogger<ProgramArgumentsParser> _logger;

        public ProgramArgumentsParser(ILogger<ProgramArgumentsParser> logger)
        {
            _logger = logger;
        }

        public ProgramArguments Parse(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("Expected at least 2 arguments");

            var testAssemblyPath = args[0];
            var outputFilePath = args[1];
            var markdownAnchor = string.Empty;
            if (args.Length == 3) {
                markdownAnchor = args[2];
            }

            if (string.IsNullOrEmpty(testAssemblyPath))
                throw new Exception("Assembly path argument invalid");

            if (string.IsNullOrEmpty(outputFilePath))
                throw new Exception("Assembly file argument invalid");

            var result = new ProgramArguments
            {
                TestAssemblyPath = testAssemblyPath,
                OutputFilePath = outputFilePath,
                MarkdownAnchor = markdownAnchor
            };
            
            _logger
                .LogInformation(result.ToString());

            return result;
        }
    }
}