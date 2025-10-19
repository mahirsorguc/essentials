using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IEssentialsLazyServiceProvider"/> that provides
/// lazy service resolution capabilities using the underlying service provider.
/// </summary>
public class EssentialsLazyServiceProvider : IEssentialsLazyServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EssentialsLazyServiceProvider"/> class.
    /// </summary>
    /// <param name="serviceProvider">The underlying service provider to resolve services from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public EssentialsLazyServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public Lazy<T> LazyGetRequiredService<T>() where T : notnull
    {
        return new Lazy<T>(() => _serviceProvider.GetRequiredService<T>());
    }

    /// <inheritdoc/>
    public Lazy<T>? LazyGetService<T>() where T : class
    {
        // Check if service exists before creating the lazy instance
        var serviceDescriptor = (_serviceProvider as ServiceProvider)?.GetService<IServiceCollection>()?
            .FirstOrDefault(sd => sd.ServiceType == typeof(T));
        
        // Alternative: Always return a Lazy, but it may throw when accessed if service doesn't exist
        return new Lazy<T>(() => _serviceProvider.GetService<T>() 
            ?? throw new InvalidOperationException($"Service of type '{typeof(T)}' is not registered."));
    }

    /// <inheritdoc/>
    public Lazy<object> LazyGetRequiredService(Type serviceType)
    {
        if (serviceType == null)
            throw new ArgumentNullException(nameof(serviceType));

        return new Lazy<object>(() => _serviceProvider.GetRequiredService(serviceType));
    }

    /// <inheritdoc/>
    public Lazy<object?>? LazyGetService(Type serviceType)
    {
        if (serviceType == null)
            throw new ArgumentNullException(nameof(serviceType));

        return new Lazy<object?>(() => _serviceProvider.GetService(serviceType));
    }
}
