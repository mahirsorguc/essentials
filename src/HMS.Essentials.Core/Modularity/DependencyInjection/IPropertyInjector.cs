namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Defines a service for injecting dependencies into object properties.
/// </summary>
public interface IPropertyInjector
{
    /// <summary>
    /// Injects dependencies into all properties marked with <see cref="InjectPropertyAttribute"/>.
    /// </summary>
    /// <param name="instance">The object instance to inject properties into.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a required property cannot be resolved from the service provider.
    /// </exception>
    void InjectProperties(object instance);

    /// <summary>
    /// Injects dependencies into all properties marked with <see cref="InjectPropertyAttribute"/> for a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="instance">The object instance to inject properties into.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a required property cannot be resolved from the service provider.
    /// </exception>
    void InjectProperties<T>(T instance) where T : class;
}
