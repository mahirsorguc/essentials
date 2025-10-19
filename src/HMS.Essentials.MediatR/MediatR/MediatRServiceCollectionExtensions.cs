using System.Reflection;
using HMS.Essentials.Modularity.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.MediatR;

/// <summary>
///     Extension methods for registering MediatR handlers discovered via reflection.
///     These helpers are optional; MediatR's RegisterServicesFromAssembly is used in the module.
/// </summary>
public static class MediatRServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRHandlers(this IServiceCollection services, Assembly assembly)
    {
        // First, register MediatR normally
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        // Then, find all handler registrations and wrap them with property injection
        var handlerRegistrations = services
            .Where(sd => sd.ServiceType.IsGenericType && 
                         (sd.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                          sd.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
            .ToList();

        foreach (var registration in handlerRegistrations)
        {
            // Remove original registration
            services.Remove(registration);
            
            // Re-add with property injection wrapper
            services.Add(ServiceDescriptor.Describe(
                registration.ServiceType,
                sp =>
                {
                    // Create instance using original implementation type
                    var instance = ActivatorUtilities.CreateInstance(sp, registration.ImplementationType!);
                    
                    // Inject properties
                    var injector = sp.GetService<IPropertyInjector>();
                    injector?.InjectProperties(instance);
                    
                    return instance;
                },
                registration.Lifetime));
        }
        
        return services;
    }
}