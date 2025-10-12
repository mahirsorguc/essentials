namespace HMS.Essentials.Modularity.Exceptions;

/// <summary>
/// Exception thrown when a module dependency is missing.
/// </summary>
public class MissingModuleDependencyException : ModuleException
{
    /// <summary>
    /// Gets the type of the missing dependency.
    /// </summary>
    public Type DependencyType { get; }

    public MissingModuleDependencyException(Type moduleType, Type dependencyType)
        : base($"Module '{moduleType.Name}' depends on '{dependencyType.Name}' which is not loaded.", moduleType)
    {
        DependencyType = dependencyType;
    }
}
