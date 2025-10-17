using HMS.Essentials.Domain;
using HMS.Essentials.EntityFrameworkCore.Configuration;
using HMS.Essentials.EntityFrameworkCore.DbContexts;
using HMS.Essentials.EntityFrameworkCore.Extensions;
using HMS.Essentials.EntityFrameworkCore.UnitOfWork;
using HMS.Essentials.Modularity;
using HMS.Essentials.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.EntityFrameworkCore;

/// <summary>
///     Entity Framework Core module for HMS Essentials.
///     Provides DbContext, Repository, and Unit of Work implementations.
/// </summary>
[DependsOn(
    typeof(EssentialsDomainModule),
    typeof(EssentialsUnitOfWorkModule)
)]
public class EssentialsEntityFrameworkCoreModule : EssentialsModule
{
    /// <summary>
    ///     Configures Entity Framework Core services.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void ConfigureServices(ModuleContext context)
    {
        var services = context.Services;

        // Register EF Core services with default configuration
        services.AddEfCoreServices(options =>
        {
            // Default configuration can be overridden by applications
            options.EnableDetailedErrors = false;
            options.EnableSensitiveDataLogging = false;
            options.MaxRetryCount = 6;
            options.MaxRetryDelay = 30;
        });

        // Register connection string provider
        services.AddSingleton<IConnectionStringProvider, DefaultConnectionStringProvider>();
    }

    /// <summary>
    ///     Initializes the Entity Framework Core module.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void Initialize(ModuleContext context)
    {
        var logger = context.ServiceProvider?.GetService<ILogger<EssentialsEntityFrameworkCoreModule>>();

        logger?.LogInformation("Entity Framework Core module initialized successfully.");

        // Check if connection string provider is registered
        var connectionStringProvider = context.ServiceProvider?.GetService<IConnectionStringProvider>();

        if (connectionStringProvider != null)
        {
            logger?.LogDebug("Connection string provider is registered and available.");
        }
        else
        {
            logger?.LogWarning("Connection string provider is not registered.");
        }
    }

    /// <summary>
    ///     Performs cleanup when the module is shutting down.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void Shutdown(ModuleContext context)
    {
        var logger = context.ServiceProvider?.GetService<ILogger<EssentialsEntityFrameworkCoreModule>>();

        logger?.LogInformation("Entity Framework Core module is shutting down.");
    }
}