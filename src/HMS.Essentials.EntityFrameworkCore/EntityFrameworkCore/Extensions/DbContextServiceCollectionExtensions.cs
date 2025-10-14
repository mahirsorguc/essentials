using HMS.Essentials.EntityFrameworkCore.Configuration;
using HMS.Essentials.EntityFrameworkCore.Repositories;
using HMS.Essentials.EntityFrameworkCore.UnitOfWork;
using HMS.Essentials.Domain.Entities;
using HMS.Essentials.Domain.Repositories;
using HMS.Essentials.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HMS.Essentials.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for registering DbContext with repositories and unit of work.
/// </summary>
public static class DbContextServiceCollectionExtensions
{
    /// <summary>
    /// Adds a DbContext with repository and unit of work support.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="optionsAction">Configuration action for DbContext options.</param>
    /// <param name="contextLifetime">The lifetime for the DbContext.</param>
    /// <param name="optionsLifetime">The lifetime for the DbContext options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEfCoreDbContext<TDbContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        // Register DbContext
        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        // Register Unit of Work
        services.TryAdd(new ServiceDescriptor(
            typeof(IUnitOfWork),
            sp => new EfCoreUnitOfWork<TDbContext>(sp.GetRequiredService<TDbContext>()),
            contextLifetime));

        return services;
    }

    /// <summary>
    /// Adds a DbContext with connection string from configuration.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="configureOptions">Additional configuration for DbContext.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEfCoreDbContext<TDbContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? configureOptions = null)
        where TDbContext : DbContext
    {
        return services.AddEfCoreDbContext<TDbContext>(options =>
        {
            configureOptions?.Invoke(options);
        });
    }

    /// <summary>
    /// Registers a repository for a specific entity.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TKey">Type of the primary key.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRepository<TDbContext, TEntity, TKey>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
        where TEntity : class, IEntity<TKey>
    {
        services.TryAdd(new ServiceDescriptor(
            typeof(IRepository<TEntity, TKey>),
            sp => new EfCoreRepository<TDbContext, TEntity, TKey>(sp.GetRequiredService<TDbContext>()),
            lifetime));

        services.TryAdd(new ServiceDescriptor(
            typeof(IReadOnlyRepository<TEntity, TKey>),
            sp => new EfCoreRepository<TDbContext, TEntity, TKey>(sp.GetRequiredService<TDbContext>()),
            lifetime));

        return services;
    }

    /// <summary>
    /// Registers a repository for a specific entity with integer key.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRepository<TDbContext, TEntity>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
        where TEntity : class, IEntity<int>
    {
        return services.AddRepository<TDbContext, TEntity, int>(lifetime);
    }

    /// <summary>
    /// Registers repositories for all entities in the DbContext.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRepositories<TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        // Use reflection to find all DbSet properties
        var dbContextType = typeof(TDbContext);
        var dbSetProperties = dbContextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var property in dbSetProperties)
        {
            var entityType = property.PropertyType.GetGenericArguments()[0];
            
            // Find IEntity<TKey> interface
            var entityInterface = entityType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType &&
                                    i.GetGenericTypeDefinition() == typeof(IEntity<>));

            if (entityInterface != null)
            {
                var keyType = entityInterface.GetGenericArguments()[0];
                
                // Register repository
                var repositoryType = typeof(IRepository<,>).MakeGenericType(entityType, keyType);
                var repositoryImplType = typeof(EfCoreRepository<,,>).MakeGenericType(dbContextType, entityType, keyType);
                
                services.TryAdd(new ServiceDescriptor(
                    repositoryType,
                    sp => Activator.CreateInstance(repositoryImplType, sp.GetRequiredService<TDbContext>())!,
                    lifetime));

                // Register read-only repository
                var readOnlyRepositoryType = typeof(IReadOnlyRepository<,>).MakeGenericType(entityType, keyType);
                
                services.TryAdd(new ServiceDescriptor(
                    readOnlyRepositoryType,
                    sp => Activator.CreateInstance(repositoryImplType, sp.GetRequiredService<TDbContext>())!,
                    lifetime));
            }
        }

        return services;
    }

    /// <summary>
    /// Configures EF Core with resilient connection using retry policy.
    /// </summary>
    /// <param name="optionsBuilder">The DbContext options builder.</param>
    /// <param name="maxRetryCount">Maximum number of retry attempts.</param>
    /// <param name="maxRetryDelay">Maximum delay between retries.</param>
    /// <returns>The DbContext options builder for chaining.</returns>
    public static DbContextOptionsBuilder UseResilientConnection(
        this DbContextOptionsBuilder optionsBuilder,
        int maxRetryCount = 6,
        TimeSpan? maxRetryDelay = null)
    {
        maxRetryDelay ??= TimeSpan.FromSeconds(30);

        return optionsBuilder;
    }
}
