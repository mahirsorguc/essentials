using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HMS.Essentials.Modularity;

/// <summary>
/// Builder for creating and configuring modular applications.
/// Provides a fluent API for setting up the module system.
/// </summary>
public sealed class ApplicationBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly ModuleLifecycleManager _lifecycleManager;
    private readonly ModuleContext _context;
    private string _environment = "Production";
    private bool _rootModuleSet = false;

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services => _services;

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration => _configuration;

    /// <summary>
    /// Gets the module context.
    /// </summary>
    public ModuleContext Context => _context;

    /// <summary>
    /// Initializes a new instance of the ApplicationBuilder class.
    /// </summary>
    public ApplicationBuilder(IServiceCollection services, IConfiguration configuration)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _context = new ModuleContext(_services, _configuration, _environment);
        _lifecycleManager = new ModuleLifecycleManager(_context);
    }

    /// <summary>
    /// Creates a new ApplicationBuilder with default configuration.
    /// </summary>
    public static ApplicationBuilder Create()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        return new ApplicationBuilder(services, configuration);
    }

    /// <summary>
    /// Sets the environment name.
    /// </summary>
    public ApplicationBuilder WithEnvironment(string environment)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        return this;
    }

    /// <summary>
    /// Uses the specified module as the root module.
    /// All dependencies will be automatically loaded.
    /// This is the recommended way to bootstrap a modular application.
    /// </summary>
    public ApplicationBuilder UseRootModule<TModule>() where TModule : IModule, new()
    {
        if (_rootModuleSet)
        {
            throw new InvalidOperationException("Root module has already been set. Only one root module is allowed.");
        }
        
        _lifecycleManager.LoadModule<TModule>();
        _rootModuleSet = true;
        return this;
    }

    /// <summary>
    /// Uses the specified module type as the root module.
    /// All dependencies will be automatically loaded.
    /// </summary>
    public ApplicationBuilder UseRootModule(Type moduleType)
    {
        if (_rootModuleSet)
        {
            throw new InvalidOperationException("Root module has already been set. Only one root module is allowed.");
        }
        
        _lifecycleManager.LoadModule(moduleType);
        _rootModuleSet = true;
        return this;
    }

    /// <summary>
    /// Discovers and adds modules from an assembly.
    /// </summary>
    public ApplicationBuilder AddModulesFromAssembly(Assembly assembly)
    {
        _lifecycleManager.LoadModulesFromAssembly(assembly);
        return this;
    }

    /// <summary>
    /// Discovers and adds modules from the calling assembly.
    /// </summary>
    public ApplicationBuilder AddModulesFromCallingAssembly()
    {
        var assembly = Assembly.GetCallingAssembly();
        _lifecycleManager.LoadModulesFromAssembly(assembly);
        return this;
    }

    /// <summary>
    /// Discovers and adds modules from the entry assembly.
    /// </summary>
    public ApplicationBuilder AddModulesFromEntryAssembly()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            _lifecycleManager.LoadModulesFromAssembly(assembly);
        }
        return this;
    }

    /// <summary>
    /// Configures services using a delegate.
    /// </summary>
    public ApplicationBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure?.Invoke(_services);
        return this;
    }

    /// <summary>
    /// Configures services using a delegate with context.
    /// </summary>
    public ApplicationBuilder ConfigureServices(Action<IServiceCollection, IConfiguration> configure)
    {
        configure?.Invoke(_services, _configuration);
        return this;
    }

    /// <summary>
    /// Builds the application and returns an ApplicationHost.
    /// </summary>
    public ApplicationHost Build()
    {
        // Configure services for all modules
        _lifecycleManager.ConfigureServices();

        // Build the service provider
        var serviceProvider = _services.BuildServiceProvider();

        // Initialize all modules
        _lifecycleManager.Initialize(serviceProvider);

        return new ApplicationHost(_lifecycleManager, serviceProvider, _context);
    }
}
