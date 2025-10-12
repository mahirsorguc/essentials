namespace HMS.Essentials.Modularity;

/// <summary>
/// Describes a module with its metadata and dependencies.
/// This class contains all information needed to load and manage a module.
/// </summary>
public sealed class ModuleDescriptor
{
    /// <summary>
    /// Gets the module type.
    /// </summary>
    public Type ModuleType { get; }

    /// <summary>
    /// Gets the module instance.
    /// </summary>
    public IModule Instance { get; internal set; }

    /// <summary>
    /// Gets the types of modules that this module depends on.
    /// </summary>
    public IReadOnlyList<Type> Dependencies { get; }

    /// <summary>
    /// Gets the module's display name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the module's description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets whether this module should be loaded on demand.
    /// </summary>
    public bool LoadOnDemand { get; }

    /// <summary>
    /// Gets the module's initialization priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Gets the current state of the module.
    /// </summary>
    public ModuleState State { get; internal set; }

    /// <summary>
    /// Gets the timestamp when the module was loaded.
    /// </summary>
    public DateTime? LoadedAt { get; internal set; }

    /// <summary>
    /// Gets the timestamp when the module was initialized.
    /// </summary>
    public DateTime? InitializedAt { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the ModuleDescriptor class.
    /// </summary>
    public ModuleDescriptor(
        Type moduleType,
        IModule instance,
        IEnumerable<Type> dependencies,
        string? name = null,
        string? description = null,
        bool loadOnDemand = false,
        int priority = 0)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        Dependencies = dependencies?.ToList() ?? new List<Type>();
        Name = name ?? moduleType.Name;
        Description = description;
        LoadOnDemand = loadOnDemand;
        Priority = priority;
        State = ModuleState.Discovered;
    }
}
