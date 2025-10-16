using System.Reflection;
using AutoMapper;
using HMS.Essentials.AutoMapper.ObjectMapping;
using HMS.Essentials.ObjectMapping;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.AutoMapper.Extensions;

/// <summary>
/// Extension methods for configuring AutoMapper services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AutoMapper services to the service collection and scans the specified assemblies for profiles.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for AutoMapper profiles.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEssentialsAutoMapper(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var assembliesToScan = assemblies.Length > 0 
            ? assemblies 
            : new[] { Assembly.GetCallingAssembly() };

        // Configure AutoMapper
        services.AddSingleton<IMapper>(provider =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assembliesToScan);
            });

            return config.CreateMapper();
        });

        // Register IObjectMapper
        services.AddSingleton<IObjectMapper, AutoMapperObjectMapper>();

        return services;
    }

    /// <summary>
    /// Adds AutoMapper services to the service collection and scans for profiles in the specified types.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="profileTypes">The types to use for finding AutoMapper profiles.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEssentialsAutoMapper(
        this IServiceCollection services,
        params Type[] profileTypes)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var assemblies = profileTypes.Length > 0
            ? profileTypes.Select(t => t.Assembly).Distinct().ToArray()
            : new[] { Assembly.GetCallingAssembly() };

        return AddEssentialsAutoMapper(services, assemblies);
    }
}
