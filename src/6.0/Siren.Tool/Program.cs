using System;
using Microsoft.Extensions.DependencyInjection;
using Siren.Application;
using Siren.Tool;

var services =
    Startup
        .AddServices();

var serviceProvider =
    services
        .BuildServiceProvider();

var application =
    serviceProvider
        .GetRequiredService<ISirenApplication>();

var exitCode =
    application
        .Perform(args);

Environment
    .Exit(exitCode);