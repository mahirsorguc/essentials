using System.Reflection;
using FluentValidation;
using HMS.Essentials.FluentValidation.AspNetCore;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.FluentValidation;

/// <summary>
/// FluentValidation module that provides comprehensive validation capabilities.
/// Automatically scans and registers validators from configured assemblies.
/// </summary>
[DependsOn(typeof(EssentialsCoreModule))]
public class EssentialsFluentValidationModule : EssentialsModule
{
    /// <summary>
    /// Configures FluentValidation services.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void ConfigureServices(ModuleContext context)
    {
        // Get configuration or use defaults
        var config = context.Configuration
            .GetSection("FluentValidation")
            .Get<FluentValidationConfiguration>() ?? new FluentValidationConfiguration();

        // Validate configuration
        config.Validate();

        // Register configuration
        context.Services.AddSingleton(config);

        // Configure FluentValidation global settings
        ConfigureValidationSettings(config);

        // Scan and register validators from configured assemblies
        if (config.AssembliesToScan.Any())
        {
            var assembliesToScan = LoadAssemblies(config.AssembliesToScan, context);
            if (assembliesToScan.Any())
            {
                // Use FluentValidation's built-in registration
                ServiceCollectionExtensions.AddValidatorsFromAssemblies(
                    context.Services, 
                    assembliesToScan, 
                    ServiceLifetime.Scoped);
            }
        }

        // Configure ASP.NET Core integration if enabled
        if (config.AspNetCore.EnableAutoValidation)
        {
            ConfigureAspNetCoreValidation(context.Services, config.AspNetCore);
        }
    }

    /// <summary>
    /// Initializes the FluentValidation module.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void Initialize(ModuleContext context)
    {
        var logger = context.GetService<ILogger<EssentialsFluentValidationModule>>();
        var config = context.GetRequiredService<FluentValidationConfiguration>();

        var validatorCount = CountRegisteredValidators(context.ServiceProvider);

        logger?.LogInformation(
            "FluentValidation module initialized successfully. Registered {ValidatorCount} validators.",
            validatorCount);

        if (config.AssembliesToScan.Any())
        {
            logger?.LogInformation(
                "Scanned assemblies: {Assemblies}",
                string.Join(", ", config.AssembliesToScan));
        }
    }

    /// <summary>
    /// Configures global FluentValidation settings.
    /// </summary>
    private void ConfigureValidationSettings(FluentValidationConfiguration config)
    {
        // Configure global validation settings
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            // Use member name as display name by default
            return member?.Name ?? expression?.ToString() ?? type.Name;
        };

        // Configure property name resolver
        ValidatorOptions.Global.PropertyNameResolver = (type, member, expression) =>
        {
            // Use member name as property name
            return member?.Name ?? expression?.ToString() ?? type.Name;
        };

        // Apply custom error messages if configured
        if (config.CustomErrorMessages.Any())
        {
            foreach (var (key, message) in config.CustomErrorMessages)
            {
                // Custom error messages can be applied here
                // This is extensible based on specific requirements
            }
        }
    }

    /// <summary>
    /// Loads assemblies by name.
    /// </summary>
    private IEnumerable<Assembly> LoadAssemblies(List<string> assemblyNames, ModuleContext context)
    {
        var assemblies = new List<Assembly>();
        var logger = context.ServiceProvider?.GetService<ILogger<EssentialsFluentValidationModule>>();

        foreach (var assemblyName in assemblyNames)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                // Log warning and continue instead of throwing
                logger?.LogWarning(ex, 
                    "Failed to load assembly '{AssemblyName}' for validator scanning. Skipping this assembly.", 
                    assemblyName);
            }
        }

        return assemblies;
    }

    /// <summary>
    /// Configures ASP.NET Core validation integration.
    /// </summary>
    private void ConfigureAspNetCoreValidation(
        IServiceCollection services,
        AspNetCoreValidationOptions options)
    {
        // Add FluentValidation for ASP.NET Core
        services.AddFluentValidationForAspNetCore(options.DisableDataAnnotationsValidation);

        // Configure client-side validation if enabled
        if (options.EnableClientSideValidation)
        {
            services.AddFluentValidationClientSide();
        }

        // Configure standardized error responses if enabled
        if (options.UseStandardErrorResponse)
        {
            services.UseStandardValidationErrorResponse(options.IncludeErrorDetails);
        }
    }

    /// <summary>
    /// Counts the number of registered validators.
    /// </summary>
    private int CountRegisteredValidators(IServiceProvider? serviceProvider)
    {
        if (serviceProvider == null)
        {
            return 0;
        }

        var validatorType = typeof(IValidator<>);
        var services = serviceProvider.GetType()
            .GetProperty("Services")?
            .GetValue(serviceProvider) as IEnumerable<ServiceDescriptor>;

        return services?
            .Count(sd => sd.ServiceType.IsGenericType && 
                         sd.ServiceType.GetGenericTypeDefinition() == validatorType) ?? 0;
    }
}