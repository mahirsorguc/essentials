using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity.Configuration;

/// <summary>
/// Provides extension methods for configuring module options.
/// </summary>
public static class ModuleConfigurationExtensions
{
    /// <summary>
    /// Configures options for a module using the configuration system.
    /// </summary>
    public static IServiceCollection ConfigureModuleOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        services.Configure<TOptions>(options => configuration.GetSection(sectionName).Bind(options));
        return services;
    }

    /// <summary>
    /// Configures options for a module using a configuration delegate.
    /// </summary>
    public static IServiceCollection ConfigureModuleOptions<TOptions>(
        this IServiceCollection services,
        Action<TOptions> configure)
        where TOptions : class
    {
        return services.Configure(configure);
    }

    /// <summary>
    /// Gets a configuration section for a module.
    /// </summary>
    public static IConfigurationSection GetModuleConfiguration(
        this IConfiguration configuration,
        string moduleName)
    {
        return configuration.GetSection($"Modules:{moduleName}");
    }

    /// <summary>
    /// Gets a configuration section for a module type.
    /// </summary>
    public static IConfigurationSection GetModuleConfiguration<TModule>(
        this IConfiguration configuration)
        where TModule : IModule
    {
        var moduleName = typeof(TModule).Name;
        if (moduleName.EndsWith("Module"))
        {
            moduleName = moduleName.Substring(0, moduleName.Length - 6);
        }
        return configuration.GetSection($"Modules:{moduleName}");
    }

    /// <summary>
    /// Binds configuration to an options object.
    /// </summary>
    public static TOptions BindModuleOptions<TOptions>(
        this IConfiguration configuration,
        string moduleName)
        where TOptions : class, new()
    {
        var options = new TOptions();
        configuration.GetSection($"Modules:{moduleName}").Bind(options);
        return options;
    }
}
