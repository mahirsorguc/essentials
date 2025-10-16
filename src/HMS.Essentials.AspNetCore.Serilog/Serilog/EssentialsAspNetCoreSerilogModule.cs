using HMS.Essentials.AspNetCore.Serilog.Configuration;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.AspNetCore.Serilog;

/// <summary>
/// Serilog module for HMS Essentials that provides structured logging capabilities.
/// This module integrates Serilog into the application with support for console and file logging,
/// log enrichment, and configuration-based setup.
/// </summary>
[DependsOn(typeof(EssentialsAspNetCoreModule))]
public class EssentialsAspNetCoreSerilogModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register Serilog options from configuration
        context.Services.Configure<SerilogOptions>(
            context.Configuration.GetSection("Serilog"));

        // Note: Actual Serilog configuration is done via WebApplicationBuilder.AddSerilogLogging()
        // extension method before the application is built, because Serilog needs to be configured
        // early in the application startup to capture all log events.
    }

    public override void Initialize(ModuleContext context)
    {
        // Log module initialization
        var loggerFactory = context.GetService<ILoggerFactory>();
        if (loggerFactory != null)
        {
            var logger = loggerFactory.CreateLogger<EssentialsAspNetCoreSerilogModule>();
            logger.LogInformation("Serilog module initialized successfully.");
        }
    }
}
