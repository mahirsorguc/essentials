using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity;

/// <summary>
/// Provides context and services to modules during their lifecycle.
/// This is the primary interface modules use to interact with the application.
/// </summary>
public sealed class ModuleContext
{
    /// <summary>
    /// Gets the service collection for registering services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the configuration root.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the service provider (available after services are built).
    /// </summary>
    public IServiceProvider? ServiceProvider { get; internal set; }

    /// <summary>
    /// Gets the collection of all module descriptors.
    /// </summary>
    public IReadOnlyList<ModuleDescriptor> Modules { get; internal set; }

    /// <summary>
    /// Gets custom properties that can be used to share data between modules.
    /// </summary>
    public IDictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the environment name (e.g., Development, Production).
    /// </summary>
    public string Environment { get; }

    /// <summary>
    /// Initializes a new instance of the ModuleContext class.
    /// </summary>
    public ModuleContext(
        IServiceCollection services,
        IConfiguration configuration,
        string environment = "Production")
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Environment = environment ?? "Production";
        Properties = new Dictionary<string, object>();
        Modules = new List<ModuleDescriptor>();
    }

    /// <summary>
    /// Gets a module descriptor by type.
    /// </summary>
    public ModuleDescriptor? GetModule<TModule>() where TModule : IModule
    {
        return Modules.FirstOrDefault(m => m.ModuleType == typeof(TModule));
    }

    /// <summary>
    /// Gets a module descriptor by type.
    /// </summary>
    public ModuleDescriptor? GetModule(Type moduleType)
    {
        return Modules.FirstOrDefault(m => m.ModuleType == moduleType);
    }

    /// <summary>
    /// Checks if a module is loaded.
    /// </summary>
    public bool IsModuleLoaded<TModule>() where TModule : IModule
    {
        var module = GetModule<TModule>();
        return module?.State >= ModuleState.Loaded;
    }

    /// <summary>
    /// Gets a required service from the service provider.
    /// </summary>
    public T GetRequiredService<T>() where T : notnull
    {
        if (ServiceProvider == null)
            throw new InvalidOperationException("Service provider is not available yet.");
        
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service from the service provider.
    /// </summary>
    public T? GetService<T>()
    {
        return ServiceProvider != null ? ServiceProvider.GetService<T>() : default;
    }
}
