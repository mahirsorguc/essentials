using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.CompilerServices;

namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Service provider decorator that automatically performs property injection
/// on all resolved services.
/// </summary>
internal class PropertyInjectingServiceProvider : IServiceProvider, ISupportRequiredService, IServiceScopeFactory
{
    private readonly IServiceProvider _inner;
    private readonly ConditionalWeakTable<object, object?> _injectedInstances = new();

    public PropertyInjectingServiceProvider(IServiceProvider inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public object? GetService(Type serviceType)
    {
        var service = _inner.GetService(serviceType);
        
        if (service != null && ShouldInjectProperties(serviceType, service))
        {
            InjectPropertiesSafe(service);
        }
        
        return service;
    }

    public object GetRequiredService(Type serviceType)
    {
        var service = (_inner as ISupportRequiredService)?.GetRequiredService(serviceType) 
                      ?? _inner.GetService(serviceType) 
                      ?? throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");
        
        if (ShouldInjectProperties(serviceType, service))
        {
            InjectPropertiesSafe(service);
        }
        
        return service;
    }

    public IServiceScope CreateScope()
    {
        var innerScopeFactory = _inner.GetRequiredService<IServiceScopeFactory>();
        var innerScope = innerScopeFactory.CreateScope();
        return new PropertyInjectingServiceScope(innerScope);
    }

    private void InjectPropertiesSafe(object service)
    {
        // Get PropertyInjector from current provider (supports both root and scoped)
        // PropertyInjector itself has caching, so we can safely call it each time
        var propertyInjector = _inner.GetService<IPropertyInjector>();
        if (propertyInjector != null)
        {
            // Check if already injected for this instance
            if (!_injectedInstances.TryGetValue(service, out _))
            {
                propertyInjector.InjectProperties(service);
                _injectedInstances.Add(service, null);
            }
        }
    }

    private static bool ShouldInjectProperties(Type serviceType, object service)
    {
        // Don't inject properties for framework types
        var serviceRuntimeType = service.GetType();
        
        if (serviceRuntimeType.Namespace?.StartsWith("Microsoft.Extensions") == true)
            return false;
            
        if (serviceRuntimeType.Namespace?.StartsWith("System") == true)
            return false;

        // Don't inject into IServiceProvider itself
        if (serviceType == typeof(IServiceProvider) || serviceRuntimeType == typeof(PropertyInjectingServiceProvider))
            return false;

        // Don't inject into IPropertyInjector to avoid circular dependency
        if (serviceType == typeof(IPropertyInjector))
            return false;

        // Don't inject into service scope factory
        if (serviceType == typeof(IServiceScopeFactory))
            return false;

        return true;
    }

    private class PropertyInjectingServiceScope : IServiceScope
    {
        private readonly IServiceScope _innerScope;
        private readonly PropertyInjectingServiceProvider _serviceProvider;

        public PropertyInjectingServiceScope(IServiceScope innerScope)
        {
            _innerScope = innerScope;
            _serviceProvider = new PropertyInjectingServiceProvider(innerScope.ServiceProvider);
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public void Dispose()
        {
            _innerScope.Dispose();
        }
    }
}
