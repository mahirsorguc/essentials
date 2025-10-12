using HMS.Essential.Logging;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Data;

/// <summary>
///     Data module that provides data access functionality.
///     This module depends on the Logging module.
/// </summary>
[DependsOn(
    typeof(LoggingEssentialsModule)
)]
public class EssentialsDataModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register data services
        context.Services.AddSingleton<IDataRepository, DataRepository>();

        Console.WriteLine("[DataModule] Services configured.");
    }

    public override void Initialize(ModuleContext context)
    {
        var logService = context.GetService<ILogService>();
        logService?.LogInfo("Data module initialized successfully.");

        Console.WriteLine("[DataModule] Initialized.");
    }

    public override void Shutdown(ModuleContext context)
    {
        Console.WriteLine("[DataModule] Shutting down...");
    }
}