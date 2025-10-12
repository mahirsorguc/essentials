using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Provides extension methods for service registration in modules.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton service with automatic interface detection.
    /// </summary>
    public static IServiceCollection AddSingletonWithInterfaces<TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
    {
        var implementationType = typeof(TImplementation);
        var interfaces = implementationType.GetInterfaces();

        services.AddSingleton<TImplementation>();

        foreach (var @interface in interfaces)
        {
            services.AddSingleton(@interface, sp => sp.GetRequiredService<TImplementation>());
        }

        return services;
    }

    /// <summary>
    /// Registers a scoped service with automatic interface detection.
    /// </summary>
    public static IServiceCollection AddScopedWithInterfaces<TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
    {
        var implementationType = typeof(TImplementation);
        var interfaces = implementationType.GetInterfaces();

        services.AddScoped<TImplementation>();

        foreach (var @interface in interfaces)
        {
            services.AddScoped(@interface, sp => sp.GetRequiredService<TImplementation>());
        }

        return services;
    }

    /// <summary>
    /// Registers a transient service with automatic interface detection.
    /// </summary>
    public static IServiceCollection AddTransientWithInterfaces<TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
    {
        var implementationType = typeof(TImplementation);
        var interfaces = implementationType.GetInterfaces();

        services.AddTransient<TImplementation>();

        foreach (var @interface in interfaces)
        {
            services.AddTransient(@interface, sp => sp.GetRequiredService<TImplementation>());
        }

        return services;
    }

    /// <summary>
    /// Replaces an existing service registration.
    /// </summary>
    public static IServiceCollection Replace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        var newDescriptor = lifetime switch
        {
            ServiceLifetime.Singleton => ServiceDescriptor.Singleton<TService, TImplementation>(),
            ServiceLifetime.Scoped => ServiceDescriptor.Scoped<TService, TImplementation>(),
            ServiceLifetime.Transient => ServiceDescriptor.Transient<TService, TImplementation>(),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };

        services.Add(newDescriptor);
        return services;
    }

    /// <summary>
    /// Adds a service only if it hasn't been registered yet.
    /// </summary>
    public static IServiceCollection TryAdd<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        if (services.Any(d => d.ServiceType == typeof(TService)))
        {
            return services;
        }

        var descriptor = lifetime switch
        {
            ServiceLifetime.Singleton => ServiceDescriptor.Singleton<TService, TImplementation>(),
            ServiceLifetime.Scoped => ServiceDescriptor.Scoped<TService, TImplementation>(),
            ServiceLifetime.Transient => ServiceDescriptor.Transient<TService, TImplementation>(),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };

        services.Add(descriptor);
        return services;
    }
}
