using HMS.Essentials;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Essential.Logging;

/// <summary>
///     Logging module that configures logging services.
/// </summary>
[DependsOn(typeof(EssentialsCoreModule))]
public class LoggingEssentialsModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Add logging services
        context.Services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register custom log service
        context.Services.AddSingleton<ILogService, LogService>();
    }

    public override void Initialize(ModuleContext context)
    {
        var logger = context.GetService<ILogger<LoggingEssentialsModule>>();
        logger?.LogInformation("Logging module initialized successfully.");
    }
}