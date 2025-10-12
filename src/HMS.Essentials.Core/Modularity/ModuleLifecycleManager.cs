using System.Reflection;
using HMS.Essentials.Modularity.Exceptions;

namespace HMS.Essentials.Modularity;

/// <summary>
/// Manages the lifecycle of modules including loading, initialization, and shutdown.
/// </summary>
public sealed class ModuleLifecycleManager
{
    private readonly List<ModuleDescriptor> _modules = new();
    private readonly ModuleContext _context;
    private bool _servicesConfigured;
    private bool _initialized;

    /// <summary>
    /// Gets the collection of loaded modules.
    /// </summary>
    public IReadOnlyList<ModuleDescriptor> Modules => _modules.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the ModuleLifecycleManager class.
    /// </summary>
    public ModuleLifecycleManager(ModuleContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Loads a module and its dependencies.
    /// </summary>
    public void LoadModule<TModule>() where TModule : IModule, new()
    {
        LoadModule(typeof(TModule));
    }

    /// <summary>
    /// Loads a module by type.
    /// </summary>
    public void LoadModule(Type moduleType)
    {
        if (moduleType == null)
            throw new ArgumentNullException(nameof(moduleType));

        if (!typeof(IModule).IsAssignableFrom(moduleType))
            throw new ModuleException($"Type '{moduleType.Name}' does not implement IModule.", moduleType);

        if (_modules.Any(m => m.ModuleType == moduleType))
            return; // Already loaded

        var descriptor = CreateModuleDescriptor(moduleType);
        
        // Load dependencies first
        foreach (var dependencyType in descriptor.Dependencies)
        {
            LoadModule(dependencyType);
        }

        descriptor.State = ModuleState.Loading;
        _modules.Add(descriptor);
        descriptor.State = ModuleState.Loaded;
        descriptor.LoadedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Discovers and loads modules from an assembly.
    /// </summary>
    public void LoadModulesFromAssembly(Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        var moduleTypes = assembly.GetTypes()
            .Where(t => typeof(IModule).IsAssignableFrom(t) && 
                       !t.IsAbstract && 
                       !t.IsInterface)
            .ToList();

        foreach (var moduleType in moduleTypes)
        {
            var attribute = moduleType.GetCustomAttribute<DependsOnAttribute>();
            if (attribute?.LoadOnDemand != true)
            {
                LoadModule(moduleType);
            }
        }
    }

    /// <summary>
    /// Configures services for all loaded modules in dependency order.
    /// </summary>
    public void ConfigureServices()
    {
        if (_servicesConfigured)
            throw new InvalidOperationException("Services have already been configured.");

        _context.Modules = _modules.AsReadOnly();

        var orderedModules = OrderModulesByDependency();

        foreach (var module in orderedModules)
        {
            try
            {
                module.State = ModuleState.ConfiguringServices;
                module.Instance.ConfigureServices(_context);
                module.State = ModuleState.ServicesConfigured;
            }
            catch (Exception ex)
            {
                module.State = ModuleState.Error;
                throw new ModuleInitializationException(
                    $"Failed to configure services for module '{module.Name}'.", 
                    module.ModuleType, 
                    ex);
            }
        }

        _servicesConfigured = true;
    }

    /// <summary>
    /// Initializes all loaded modules in dependency order.
    /// </summary>
    public void Initialize(IServiceProvider serviceProvider)
    {
        if (!_servicesConfigured)
            throw new InvalidOperationException("Services must be configured before initialization.");

        if (_initialized)
            throw new InvalidOperationException("Modules have already been initialized.");

        _context.ServiceProvider = serviceProvider;

        var orderedModules = OrderModulesByDependency();

        foreach (var module in orderedModules)
        {
            try
            {
                module.State = ModuleState.Initializing;
                module.Instance.Initialize(_context);
                module.State = ModuleState.Initialized;
                module.InitializedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                module.State = ModuleState.Error;
                throw new ModuleInitializationException(
                    $"Failed to initialize module '{module.Name}'.", 
                    module.ModuleType, 
                    ex);
            }
        }

        _initialized = true;
    }

    /// <summary>
    /// Shuts down all modules in reverse dependency order.
    /// </summary>
    public void Shutdown()
    {
        var orderedModules = OrderModulesByDependency().Reverse();

        foreach (var module in orderedModules)
        {
            if (module.State == ModuleState.Initialized)
            {
                try
                {
                    module.State = ModuleState.ShuttingDown;
                    module.Instance.Shutdown(_context);
                    module.State = ModuleState.ShutDown;
                }
                catch (Exception ex)
                {
                    module.State = ModuleState.Error;
                    // Log but don't throw during shutdown
                    Console.Error.WriteLine($"Error shutting down module '{module.Name}': {ex.Message}");
                }
            }
        }
    }

    private ModuleDescriptor CreateModuleDescriptor(Type moduleType)
    {
        var instance = (IModule)Activator.CreateInstance(moduleType)!;
        var attribute = moduleType.GetCustomAttribute<DependsOnAttribute>();

        var dependencies = attribute?.DependsOn ?? Array.Empty<Type>();
        
        return new ModuleDescriptor(
            moduleType,
            instance,
            dependencies,
            attribute?.Name,
            attribute?.Description,
            attribute?.LoadOnDemand ?? false,
            attribute?.Priority ?? 0);
    }

    private IEnumerable<ModuleDescriptor> OrderModulesByDependency()
    {
        var resolver = new ModuleDependencyResolver(_modules);
        return resolver.ResolveDependencyOrder();
    }
}
