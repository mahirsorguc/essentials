namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Provides lazy service resolution capabilities for dependency injection.
/// This interface allows services to be resolved on-demand rather than at construction time.
/// </summary>
public interface IEssentialsLazyServiceProvider
{
    /// <summary>
    /// Lazily resolves a service of the specified type.
    /// The service will be resolved from the service provider only when the Lazy value is accessed.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>A Lazy instance that will resolve the service on first access.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service cannot be resolved from the service provider.
    /// </exception>
    Lazy<T> LazyGetRequiredService<T>() where T : notnull;

    /// <summary>
    /// Lazily resolves a service of the specified type.
    /// The service will be resolved from the service provider only when the Lazy value is accessed.
    /// Returns null if the service is not registered.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>A Lazy instance that will resolve the service on first access, or null if not registered.</returns>
    Lazy<T>? LazyGetService<T>() where T : class;

    /// <summary>
    /// Lazily resolves a service of the specified type.
    /// The service will be resolved from the service provider only when the Lazy value is accessed.
    /// </summary>
    /// <param name="serviceType">The type of service to resolve.</param>
    /// <returns>A Lazy instance that will resolve the service on first access.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceType"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service cannot be resolved from the service provider.
    /// </exception>
    Lazy<object> LazyGetRequiredService(Type serviceType);

    /// <summary>
    /// Lazily resolves a service of the specified type.
    /// The service will be resolved from the service provider only when the Lazy value is accessed.
    /// Returns null if the service is not registered.
    /// </summary>
    /// <param name="serviceType">The type of service to resolve.</param>
    /// <returns>A Lazy instance that will resolve the service on first access, or null if not registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceType"/> is null.</exception>
    Lazy<object?>? LazyGetService(Type serviceType);
}
