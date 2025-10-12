namespace HMS.Essentials.Modularity;

/// <summary>
/// Represents the current state of a module in its lifecycle.
/// </summary>
public enum ModuleState
{
    /// <summary>
    /// Module has been discovered but not yet loaded.
    /// </summary>
    Discovered,

    /// <summary>
    /// Module is currently being loaded.
    /// </summary>
    Loading,

    /// <summary>
    /// Module has been loaded successfully.
    /// </summary>
    Loaded,

    /// <summary>
    /// Module services are being configured.
    /// </summary>
    ConfiguringServices,

    /// <summary>
    /// Module services have been configured.
    /// </summary>
    ServicesConfigured,

    /// <summary>
    /// Module is being initialized.
    /// </summary>
    Initializing,

    /// <summary>
    /// Module has been initialized and is ready.
    /// </summary>
    Initialized,

    /// <summary>
    /// Module is shutting down.
    /// </summary>
    ShuttingDown,

    /// <summary>
    /// Module has been shut down.
    /// </summary>
    ShutDown,

    /// <summary>
    /// Module encountered an error during lifecycle.
    /// </summary>
    Error
}
