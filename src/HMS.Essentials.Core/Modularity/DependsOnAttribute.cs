namespace HMS.Essentials.Modularity;

/// <summary>
/// Attribute to define module dependencies and metadata.
/// This allows declarative specification of module relationships.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DependsOnAttribute : Attribute
{
    /// <summary>
    /// Gets the types of modules that this module depends on.
    /// Dependencies will be loaded and initialized before this module.
    /// </summary>
    public Type[] DependsOn { get; }

    /// <summary>
    /// Gets or sets the module's display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the module's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this module should be loaded on demand rather than automatically.
    /// </summary>
    public bool LoadOnDemand { get; set; }

    /// <summary>
    /// Gets or sets the module's initialization priority.
    /// Higher values are initialized first. Default is 0.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Initializes a new instance of the ModuleAttribute class.
    /// </summary>
    /// <param name="dependsOn">The types of modules that this module depends on.</param>
    public DependsOnAttribute(params Type[] dependsOn)
    {
        DependsOn = dependsOn ?? Array.Empty<Type>();
    }
}
