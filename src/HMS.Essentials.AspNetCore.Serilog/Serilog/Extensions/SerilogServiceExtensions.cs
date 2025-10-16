using HMS.Essentials.AspNetCore.Serilog.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace HMS.Essentials.AspNetCore.Serilog.Extensions;

/// <summary>
/// Extension methods for configuring Serilog in ASP.NET Core applications.
/// </summary>
public static class SerilogServiceExtensions
{
    /// <summary>
    /// Adds Serilog logging to the WebApplicationBuilder using configuration from appsettings.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <param name="configure">Optional action to configure SerilogOptions.</param>
    /// <returns>The WebApplicationBuilder for method chaining.</returns>
    public static WebApplicationBuilder AddSerilogLogging(
        this WebApplicationBuilder builder,
        Action<SerilogOptions>? configure = null)
    {
        // Get Serilog options from configuration
        var serilogOptions = builder.Configuration
            .GetSection("Serilog")
            .Get<SerilogOptions>() ?? new SerilogOptions();

        // Allow programmatic configuration
        configure?.Invoke(serilogOptions);

        // Register options in DI container
        builder.Services.Configure<SerilogOptions>(options =>
        {
            options.Enabled = serilogOptions.Enabled;
            options.MinimumLevel = serilogOptions.MinimumLevel;
            options.WriteToConsole = serilogOptions.WriteToConsole;
            options.WriteToFile = serilogOptions.WriteToFile;
            options.LogFilePath = serilogOptions.LogFilePath;
            options.RollingInterval = serilogOptions.RollingInterval;
            options.RetainedFileCountLimit = serilogOptions.RetainedFileCountLimit;
            options.FileSizeLimitBytes = serilogOptions.FileSizeLimitBytes;
            options.EnrichWithMachineName = serilogOptions.EnrichWithMachineName;
            options.EnrichWithThreadId = serilogOptions.EnrichWithThreadId;
            options.EnrichWithEnvironmentName = serilogOptions.EnrichWithEnvironmentName;
            options.ConsoleOutputTemplate = serilogOptions.ConsoleOutputTemplate;
            options.FileOutputTemplate = serilogOptions.FileOutputTemplate;
            options.OverrideMinimumLevel = serilogOptions.OverrideMinimumLevel;
            options.MinimumLevelOverrides = serilogOptions.MinimumLevelOverrides;
        });

        if (!serilogOptions.Enabled)
        {
            return builder;
        }

        // Configure Serilog
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration);

        // Apply minimum level
        if (Enum.TryParse<LogEventLevel>(serilogOptions.MinimumLevel, true, out var minLevel))
        {
            loggerConfiguration.MinimumLevel.Is(minLevel);
        }

        // Apply minimum level overrides
        foreach (var (source, level) in serilogOptions.MinimumLevelOverrides)
        {
            if (Enum.TryParse<LogEventLevel>(level, true, out var overrideLevel))
            {
                loggerConfiguration.MinimumLevel.Override(source, overrideLevel);
            }
        }

        // Enrich logs
        loggerConfiguration.Enrich.FromLogContext();

        if (serilogOptions.EnrichWithMachineName)
        {
            loggerConfiguration.Enrich.WithMachineName();
        }

        if (serilogOptions.EnrichWithThreadId)
        {
            loggerConfiguration.Enrich.WithThreadId();
        }

        if (serilogOptions.EnrichWithEnvironmentName)
        {
            loggerConfiguration.Enrich.WithEnvironmentName();
        }

        // Add console sink
        if (serilogOptions.WriteToConsole)
        {
            if (!string.IsNullOrWhiteSpace(serilogOptions.ConsoleOutputTemplate))
            {
                loggerConfiguration.WriteTo.Console(
                    outputTemplate: serilogOptions.ConsoleOutputTemplate);
            }
            else
            {
                loggerConfiguration.WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
            }
        }

        // Add file sink
        if (serilogOptions.WriteToFile)
        {
            var rollingInterval = ParseRollingInterval(serilogOptions.RollingInterval);

            if (!string.IsNullOrWhiteSpace(serilogOptions.FileOutputTemplate))
            {
                loggerConfiguration.WriteTo.File(
                    path: serilogOptions.LogFilePath,
                    rollingInterval: rollingInterval,
                    retainedFileCountLimit: serilogOptions.RetainedFileCountLimit,
                    fileSizeLimitBytes: serilogOptions.FileSizeLimitBytes,
                    outputTemplate: serilogOptions.FileOutputTemplate);
            }
            else
            {
                loggerConfiguration.WriteTo.File(
                    path: serilogOptions.LogFilePath,
                    rollingInterval: rollingInterval,
                    retainedFileCountLimit: serilogOptions.RetainedFileCountLimit,
                    fileSizeLimitBytes: serilogOptions.FileSizeLimitBytes,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
            }
        }

        // Create logger
        Log.Logger = loggerConfiguration.CreateLogger();

        // Use Serilog for ASP.NET Core logging
        builder.Host.UseSerilog(Log.Logger, dispose: true);

        return builder;
    }

    /// <summary>
    /// Adds Serilog request logging middleware.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The WebApplication for method chaining.</returns>
    public static WebApplication UseEssentialsSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : LogEventLevel.Information;
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            };
        });

        return app;
    }

    private static RollingInterval ParseRollingInterval(string interval)
    {
        return interval?.ToLowerInvariant() switch
        {
            "infinite" => RollingInterval.Infinite,
            "year" => RollingInterval.Year,
            "month" => RollingInterval.Month,
            "day" => RollingInterval.Day,
            "hour" => RollingInterval.Hour,
            "minute" => RollingInterval.Minute,
            _ => RollingInterval.Day
        };
    }
}
