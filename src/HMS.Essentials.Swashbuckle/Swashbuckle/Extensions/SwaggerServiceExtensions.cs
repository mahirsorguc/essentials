using HMS.Essentials.Swashbuckle.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace HMS.Essentials.Swashbuckle.Extensions;

/// <summary>
/// Extension methods for registering Swagger services.
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Adds Swagger services with default configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        return services.AddSwaggerServices(options => { });
    }

    /// <summary>
    /// Adds Swagger services with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure Swagger options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwaggerServices(
        this IServiceCollection services,
        Action<SwaggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new SwaggerOptions();
        configure(options);

        // Register the options
        services.AddSingleton(options);

        // Add API Explorer services (required for Swagger)
        services.AddEndpointsApiExplorer();

        // Configure Swagger Gen
        services.AddSwaggerGen(swaggerOptions =>
        {
            ConfigureSwaggerGen(swaggerOptions, options);
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger Gen with the provided options.
    /// </summary>
    private static void ConfigureSwaggerGen(SwaggerGenOptions swaggerOptions, SwaggerOptions options)
    {
        // Configure OpenAPI info
        swaggerOptions.SwaggerDoc(options.Version, CreateOpenApiInfo(options));

        // Include XML comments if configured
        if (options.IncludeXmlComments)
        {
            IncludeXmlComments(swaggerOptions);
        }

        // Custom schema IDs to avoid conflicts
        swaggerOptions.CustomSchemaIds(type => type.FullName);
    }

    /// <summary>
    /// Creates OpenAPI info from options.
    /// </summary>
    private static OpenApiInfo CreateOpenApiInfo(SwaggerOptions options)
    {
        var info = new OpenApiInfo
        {
            Title = options.Title,
            Version = options.Version,
            Description = options.Description
        };

        // Add terms of service if provided
        if (!string.IsNullOrWhiteSpace(options.TermsOfServiceUrl))
        {
            info.TermsOfService = new Uri(options.TermsOfServiceUrl);
        }

        // Add contact if provided
        if (!string.IsNullOrWhiteSpace(options.ContactName) ||
            !string.IsNullOrWhiteSpace(options.ContactEmail) ||
            !string.IsNullOrWhiteSpace(options.ContactUrl))
        {
            info.Contact = new OpenApiContact
            {
                Name = options.ContactName,
                Email = options.ContactEmail,
                Url = !string.IsNullOrWhiteSpace(options.ContactUrl)
                    ? new Uri(options.ContactUrl)
                    : null
            };
        }

        // Add license if provided
        if (!string.IsNullOrWhiteSpace(options.LicenseName))
        {
            info.License = new OpenApiLicense
            {
                Name = options.LicenseName,
                Url = !string.IsNullOrWhiteSpace(options.LicenseUrl)
                    ? new Uri(options.LicenseUrl)
                    : null
            };
        }

        return info;
    }

    /// <summary>
    /// Includes XML comments from all assemblies in the current domain.
    /// </summary>
    private static void IncludeXmlComments(SwaggerGenOptions swaggerOptions)
    {
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            // Skip dynamic assemblies and system assemblies
            if (assembly.IsDynamic)
                continue;

            try
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    swaggerOptions.IncludeXmlComments(xmlPath);
                }
            }
            catch
            {
                // Ignore errors for assemblies that don't have XML documentation
            }
        }
    }
}
