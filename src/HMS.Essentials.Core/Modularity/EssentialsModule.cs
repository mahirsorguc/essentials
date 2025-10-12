namespace HMS.Essentials.Modularity;

/// <summary>
/// Base class for modules providing default implementations.
/// Modules can inherit from this class for convenience.
/// </summary>
public abstract class EssentialsModule : IModule
{
    /// <summary>
    /// Called during the service registration phase.
    /// Override this method to register services.
    /// </summary>
    public virtual void ConfigureServices(ModuleContext context)
    {
        // Default implementation - do nothing
    }

    /// <summary>
    /// Called after all modules have registered their services.
    /// Override this method to perform initialization.
    /// </summary>
    public virtual void Initialize(ModuleContext context)
    {
        // Default implementation - do nothing
    }

    /// <summary>
    /// Called when the application is shutting down.
    /// Override this method to perform cleanup.
    /// </summary>
    public virtual void Shutdown(ModuleContext context)
    {
        // Default implementation - do nothing
    }
}
