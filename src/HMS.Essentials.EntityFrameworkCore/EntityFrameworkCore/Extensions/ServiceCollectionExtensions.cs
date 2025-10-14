using HMS.Essentials.EntityFrameworkCore.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HMS.Essentials.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for registering Entity Framework Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Entity Framework Core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for EF Core options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEfCoreServices(
        this IServiceCollection services,
        Action<EfCoreOptions>? configure = null)
    {
        var options = new EfCoreOptions();
        configure?.Invoke(options);

        services.Configure<EfCoreOptions>(efCoreOptions =>
        {
            efCoreOptions.DefaultConnectionString = options.DefaultConnectionString;
            efCoreOptions.ConnectionStrings = options.ConnectionStrings;
            efCoreOptions.EnableDetailedErrors = options.EnableDetailedErrors;
            efCoreOptions.EnableSensitiveDataLogging = options.EnableSensitiveDataLogging;
            efCoreOptions.MaxRetryCount = options.MaxRetryCount;
            efCoreOptions.MaxRetryDelay = options.MaxRetryDelay;
            efCoreOptions.UseLazyLoadingProxies = options.UseLazyLoadingProxies;
            efCoreOptions.CommandTimeout = options.CommandTimeout;
            efCoreOptions.AutoMigrateOnStartup = options.AutoMigrateOnStartup;
            efCoreOptions.SeedDataOnStartup = options.SeedDataOnStartup;
        });

        // Register connection string provider
        var connectionStringProvider = new DefaultConnectionStringProvider(options.DefaultConnectionString);
        foreach (var kvp in options.ConnectionStrings)
        {
            connectionStringProvider.AddConnectionString(kvp.Key, kvp.Value);
        }
        
        services.TryAddSingleton<IConnectionStringProvider>(connectionStringProvider);

        return services;
    }

    /// <summary>
    /// Adds a connection string for a specific DbContext.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contextName">The name of the DbContext.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConnectionString(
        this IServiceCollection services,
        string contextName,
        string connectionString)
    {
        var serviceProvider = services.BuildServiceProvider();
        var connectionStringProvider = serviceProvider.GetService<IConnectionStringProvider>();

        if (connectionStringProvider is DefaultConnectionStringProvider defaultProvider)
        {
            defaultProvider.AddConnectionString(contextName, connectionString);
        }

        return services;
    }
}
