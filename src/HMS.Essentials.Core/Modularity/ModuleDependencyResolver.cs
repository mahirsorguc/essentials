using HMS.Essentials.Modularity.Exceptions;

namespace HMS.Essentials.Modularity;

/// <summary>
/// Resolves module dependencies and determines the correct initialization order.
/// Uses topological sorting to handle dependency graphs.
/// </summary>
internal sealed class ModuleDependencyResolver
{
    private readonly IReadOnlyList<ModuleDescriptor> _modules;

    public ModuleDependencyResolver(IReadOnlyList<ModuleDescriptor> modules)
    {
        _modules = modules ?? throw new ArgumentNullException(nameof(modules));
    }

    /// <summary>
    /// Resolves the dependency order using topological sorting.
    /// Returns modules in the order they should be initialized.
    /// </summary>
    public IEnumerable<ModuleDescriptor> ResolveDependencyOrder()
    {
        ValidateDependencies();
        
        var visited = new HashSet<Type>();
        var visiting = new HashSet<Type>();
        var result = new List<ModuleDescriptor>();

        // First, sort by priority (higher priority first)
        var prioritySorted = _modules.OrderByDescending(m => m.Priority).ToList();

        foreach (var module in prioritySorted)
        {
            if (!visited.Contains(module.ModuleType))
            {
                VisitModule(module, visited, visiting, result, new List<Type>());
            }
        }

        return result;
    }

    private void VisitModule(
        ModuleDescriptor module,
        HashSet<Type> visited,
        HashSet<Type> visiting,
        List<ModuleDescriptor> result,
        List<Type> path)
    {
        if (visiting.Contains(module.ModuleType))
        {
            // Circular dependency detected
            path.Add(module.ModuleType);
            throw new CircularDependencyException(path);
        }

        if (visited.Contains(module.ModuleType))
        {
            return;
        }

        visiting.Add(module.ModuleType);
        path.Add(module.ModuleType);

        // Visit all dependencies first
        foreach (var dependencyType in module.Dependencies)
        {
            var dependencyModule = _modules.FirstOrDefault(m => m.ModuleType == dependencyType);
            
            if (dependencyModule == null)
            {
                throw new MissingModuleDependencyException(module.ModuleType, dependencyType);
            }

            VisitModule(dependencyModule, visited, visiting, result, new List<Type>(path));
        }

        visiting.Remove(module.ModuleType);
        visited.Add(module.ModuleType);
        result.Add(module);
    }

    private void ValidateDependencies()
    {
        foreach (var module in _modules)
        {
            foreach (var dependencyType in module.Dependencies)
            {
                if (!typeof(IModule).IsAssignableFrom(dependencyType))
                {
                    throw new ModuleException(
                        $"Module '{module.ModuleType.Name}' declares a dependency on '{dependencyType.Name}' which is not a valid module type.",
                        module.ModuleType);
                }

                var dependencyExists = _modules.Any(m => m.ModuleType == dependencyType);
                if (!dependencyExists)
                {
                    throw new MissingModuleDependencyException(module.ModuleType, dependencyType);
                }
            }
        }
    }

    /// <summary>
    /// Gets all dependencies of a module recursively.
    /// </summary>
    public IEnumerable<Type> GetAllDependencies(Type moduleType)
    {
        var module = _modules.FirstOrDefault(m => m.ModuleType == moduleType);
        if (module == null)
        {
            return Enumerable.Empty<Type>();
        }

        var dependencies = new HashSet<Type>();
        CollectDependencies(module, dependencies);
        return dependencies;
    }

    private void CollectDependencies(ModuleDescriptor module, HashSet<Type> dependencies)
    {
        foreach (var dependencyType in module.Dependencies)
        {
            if (dependencies.Add(dependencyType))
            {
                var dependencyModule = _modules.FirstOrDefault(m => m.ModuleType == dependencyType);
                if (dependencyModule != null)
                {
                    CollectDependencies(dependencyModule, dependencies);
                }
            }
        }
    }
}
