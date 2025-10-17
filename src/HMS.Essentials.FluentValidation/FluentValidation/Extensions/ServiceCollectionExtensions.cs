using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.FluentValidation.Extensions;

/// <summary>
/// Extension methods for registering FluentValidation validators with the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the specified assembly and registers all validators with the service collection.
    /// Validators are registered with Scoped lifetime by default.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly to scan for validators.</param>
    /// <param name="lifetime">The service lifetime for validators (default: Scoped).</param>
    /// <param name="filter">Optional filter to include/exclude validators.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddValidatorsFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var scanResults = AssemblyScanner.FindValidatorsInAssembly(assembly);
        
        foreach (var result in scanResults)
        {
            if (filter != null && !filter(result))
            {
                continue;
            }

            services.Add(
                ServiceDescriptor.Describe(
                    result.InterfaceType,
                    result.ValidatorType,
                    lifetime));
        }

        return services;
    }

    /// <summary>
    /// Scans multiple assemblies and registers all validators with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for validators.</param>
    /// <param name="lifetime">The service lifetime for validators (default: Scoped).</param>
    /// <param name="filter">Optional filter to include/exclude validators.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddValidatorsFromAssemblies(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly, lifetime, filter);
        }

        return services;
    }

    /// <summary>
    /// Scans the assembly containing the specified type and registers all validators.
    /// </summary>
    /// <typeparam name="T">Type in the assembly to scan.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime for validators (default: Scoped).</param>
    /// <param name="filter">Optional filter to include/exclude validators.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null)
    {
        return services.AddValidatorsFromAssembly(
            typeof(T).Assembly,
            lifetime,
            filter);
    }

    /// <summary>
    /// Registers a specific validator with the service collection.
    /// </summary>
    /// <typeparam name="TValidator">The validator type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddValidator<TValidator>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TValidator : class, IValidator
    {
        ArgumentNullException.ThrowIfNull(services);

        var validatorType = typeof(TValidator);
        var interfaceType = validatorType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

        if (interfaceType == null)
        {
            throw new InvalidOperationException(
                $"The validator type '{validatorType.Name}' does not implement IValidator<T>.");
        }

        services.Add(ServiceDescriptor.Describe(interfaceType, validatorType, lifetime));
        return services;
    }

    /// <summary>
    /// Registers a specific validator with the service collection using explicit interface and implementation types.
    /// </summary>
    /// <typeparam name="TInterface">The validator interface type (IValidator&lt;T&gt;).</typeparam>
    /// <typeparam name="TValidator">The validator implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddValidator<TInterface, TValidator>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class, IValidator
        where TValidator : class, TInterface
    {
        ArgumentNullException.ThrowIfNull(services);
        services.Add(ServiceDescriptor.Describe(typeof(TInterface), typeof(TValidator), lifetime));
        return services;
    }

    /// <summary>
    /// Configures global FluentValidation options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure validation options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection ConfigureFluentValidation(
        this IServiceCollection services,
        Action<FluentValidationConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var config = new FluentValidationConfiguration();
        configure(config);

        services.AddSingleton(config);
        
        return services;
    }
}
