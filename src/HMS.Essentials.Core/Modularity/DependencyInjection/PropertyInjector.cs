using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IPropertyInjector"/> that injects dependencies into properties
/// marked with <see cref="InjectPropertyAttribute"/>.
/// </summary>
public class PropertyInjector : IPropertyInjector
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();
    private static readonly object _cacheLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyInjector"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public PropertyInjector(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public void InjectProperties(object instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        var type = instance.GetType();
        var properties = GetInjectableProperties(type);

        foreach (var property in properties)
        {
            InjectProperty(instance, property);
        }
    }

    /// <inheritdoc/>
    public void InjectProperties<T>(T instance) where T : class
    {
        InjectProperties((object)instance);
    }

    private void InjectProperty(object instance, PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<InjectPropertyAttribute>();
        if (attribute == null)
            return;

        // Check if property is settable
        if (!property.CanWrite)
        {
            throw new InvalidOperationException(
                $"Property '{property.Name}' on type '{instance.GetType().Name}' is marked for injection but is not settable.");
        }

        object? service;

        try
        {
            // Resolve the service from the service provider
            if (attribute.ServiceKey != null)
            {
                // For keyed services (future .NET support)
                service = _serviceProvider.GetService(property.PropertyType);
            }
            else
            {
                service = _serviceProvider.GetService(property.PropertyType);
            }
        }
        catch (Exception ex)
        {
            if (attribute.Required)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve required property '{property.Name}' of type '{property.PropertyType.Name}' " +
                    $"on instance of type '{instance.GetType().Name}'.", ex);
            }
            return;
        }

        // Set the property value
        if (service != null)
        {
            property.SetValue(instance, service);
        }
        else if (attribute.Required)
        {
            throw new InvalidOperationException(
                $"Required property '{property.Name}' of type '{property.PropertyType.Name}' " +
                $"on instance of type '{instance.GetType().Name}' could not be resolved from the service provider.");
        }
    }

    private static PropertyInfo[] GetInjectableProperties(Type type)
    {
        // Check cache first
        if (_propertyCache.TryGetValue(type, out var cachedProperties))
        {
            return cachedProperties;
        }

        lock (_cacheLock)
        {
            // Double-check after acquiring lock
            if (_propertyCache.TryGetValue(type, out cachedProperties))
            {
                return cachedProperties;
            }

            // Get all instance properties with the InjectProperty attribute
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<InjectPropertyAttribute>() != null)
                .ToArray();

            _propertyCache[type] = properties;
            return properties;
        }
    }
}
