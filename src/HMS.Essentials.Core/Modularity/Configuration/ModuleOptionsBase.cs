namespace HMS.Essentials.Modularity.Configuration;

/// <summary>
/// Base class for module configuration options.
/// </summary>
public abstract class ModuleOptionsBase
{
    /// <summary>
    /// Gets or sets whether the module is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets custom properties for the module.
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = new();

    /// <summary>
    /// Validates the configuration options.
    /// Override this method to implement custom validation logic.
    /// </summary>
    public virtual void Validate()
    {
        // Base implementation - no validation
    }
}
