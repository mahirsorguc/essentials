namespace HMS.Essentials.Modularity.Exceptions;

/// <summary>
/// Exception thrown when a circular dependency is detected in module dependencies.
/// </summary>
public class CircularDependencyException : ModuleException
{
    /// <summary>
    /// Gets the chain of types that form the circular dependency.
    /// </summary>
    public IReadOnlyList<Type> DependencyChain { get; }

    public CircularDependencyException(IEnumerable<Type> dependencyChain)
        : base($"Circular dependency detected: {string.Join(" -> ", dependencyChain.Select(t => t.Name))}")
    {
        DependencyChain = dependencyChain.ToList();
    }
}
