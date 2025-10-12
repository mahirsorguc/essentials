namespace HMS.Essentials.Modularity;

/// <summary>
/// Defines the contract for a module in the HMS Essentials modularity system.
/// All modules must implement this interface to participate in the module lifecycle.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Called during the service registration phase.
    /// Register all required services, configurations, and dependencies here.
    /// </summary>
    /// <param name="context">The module context providing access to service registration and configuration.</param>
    void ConfigureServices(ModuleContext context);

    /// <summary>
    /// Called after all modules have registered their services.
    /// Use this to configure middleware, set up application behavior, or perform initialization.
    /// </summary>
    /// <param name="context">The module context providing access to the configured application.</param>
    void Initialize(ModuleContext context);

    /// <summary>
    /// Called when the application is shutting down.
    /// Clean up resources, close connections, and perform cleanup operations.
    /// </summary>
    /// <param name="context">The module context for cleanup operations.</param>
    void Shutdown(ModuleContext context);
}
