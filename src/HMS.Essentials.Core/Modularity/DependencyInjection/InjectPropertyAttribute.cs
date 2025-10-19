namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Marks a property for automatic dependency injection.
/// Properties marked with this attribute will be automatically resolved from the service provider.
/// </summary>
/// <remarks>
/// <para>
/// Property injection is an alternative to constructor injection that can be useful in scenarios where:
/// - The dependency is optional
/// - The class has a large number of dependencies
/// - The class is instantiated by a framework that doesn't support constructor injection
/// </para>
/// <para>
/// Note: Constructor injection is generally preferred over property injection as it makes dependencies explicit
/// and ensures the object is fully initialized when constructed.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class MyService
/// {
///     [InjectProperty]
///     public ILogger&lt;MyService&gt; Logger { get; set; } = null!;
///     
///     [InjectProperty(Required = false)]
///     public IOptionalService? OptionalService { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class InjectPropertyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether the property is required.
    /// If true, an exception will be thrown if the service cannot be resolved.
    /// If false, the property will be set to null if the service is not found.
    /// </summary>
    /// <value>
    /// <c>true</c> if the property is required; otherwise, <c>false</c>.
    /// Default is <c>true</c>.
    /// </value>
    public bool Required { get; set; } = true;

    /// <summary>
    /// Gets or sets the service key for keyed services (optional).
    /// </summary>
    /// <value>
    /// The service key to use when resolving the service, or null for non-keyed services.
    /// </value>
    public object? ServiceKey { get; set; }
}
