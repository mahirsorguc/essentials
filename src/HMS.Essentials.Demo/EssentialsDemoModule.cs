using HMS.Essentials.Data;
using HMS.Essentials.Modularity;

namespace HMS.Essentials;

/// <summary>
/// Root module for the demo application.
/// This module depends on Data module (which transitively depends on Logging module).
/// </summary>
[DependsOn(typeof(EssentialsDataModule))]
public class EssentialsDemoModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        Console.WriteLine("[DemoModule] Configuring services...");
        // Add any demo-specific services here
    }

    public override void Initialize(ModuleContext context)
    {
        Console.WriteLine("[DemoModule] Initialized successfully.");
    }

    public override void Shutdown(ModuleContext context)
    {
        Console.WriteLine("[DemoModule] Shutting down...");
    }
}