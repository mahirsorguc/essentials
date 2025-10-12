using HMS.Essentials.Domain.Entities;
using HMS.Essentials.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Domain.Extensions;

/// <summary>
/// Extension methods for registering repositories in the dependency injection container.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    /// <summary>
    /// Registers an in-memory repository for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInMemoryRepository<TEntity, TKey>(this IServiceCollection services)
        where TEntity : class, IEntity<TKey>
    {
        services.AddSingleton<IRepository<TEntity, TKey>, InMemoryRepository<TEntity, TKey>>();
        services.AddSingleton<IReadOnlyRepository<TEntity, TKey>>(sp => 
            sp.GetRequiredService<IRepository<TEntity, TKey>>());
        
        return services;
    }

    /// <summary>
    /// Registers an in-memory repository for the specified entity type with integer key.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInMemoryRepository<TEntity>(this IServiceCollection services)
        where TEntity : class, IEntity<int>
    {
        return services.AddInMemoryRepository<TEntity, int>();
    }

    /// <summary>
    /// Registers a custom repository implementation.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    /// <typeparam name="TRepository">The repository implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRepository<TEntity, TKey, TRepository>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEntity : class, IEntity<TKey>
        where TRepository : class, IRepository<TEntity, TKey>
    {
        services.Add(new ServiceDescriptor(
            typeof(IRepository<TEntity, TKey>),
            typeof(TRepository),
            lifetime));

        services.Add(new ServiceDescriptor(
            typeof(IReadOnlyRepository<TEntity, TKey>),
            sp => sp.GetRequiredService<IRepository<TEntity, TKey>>(),
            lifetime));

        return services;
    }

    /// <summary>
    /// Registers a custom read-only repository implementation.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    /// <typeparam name="TRepository">The repository implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddReadOnlyRepository<TEntity, TKey, TRepository>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEntity : class, IEntity<TKey>
        where TRepository : class, IReadOnlyRepository<TEntity, TKey>
    {
        services.Add(new ServiceDescriptor(
            typeof(IReadOnlyRepository<TEntity, TKey>),
            typeof(TRepository),
            lifetime));

        return services;
    }
}
