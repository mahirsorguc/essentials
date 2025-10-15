using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle.Configuration;
using HMS.Essentials.Swashbuckle.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.Swashbuckle;

/// <summary>
/// Swashbuckle module that configures Swagger/OpenAPI documentation.
/// Provides automatic API documentation generation with customizable options.
/// </summary>
[DependsOn(typeof(EssentialsCoreModule))]
public class EssentialsSwashbuckleModule : EssentialsModule
{
    /// <summary>
    /// Configures Swagger services.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void ConfigureServices(ModuleContext context)
    {
        // Get Swagger options from configuration or use defaults
        var swaggerOptions = context.Configuration
            .GetSection("Swagger")
            .Get<SwaggerOptions>() ?? new SwaggerOptions();

        // Register Swagger services with the configured options
        context.Services.AddSwaggerServices(options =>
        {
            options.EnableSwaggerUI = swaggerOptions.EnableSwaggerUI;
            options.EnableInProduction = swaggerOptions.EnableInProduction;
            options.Title = swaggerOptions.Title;
            options.Version = swaggerOptions.Version;
            options.Description = swaggerOptions.Description;
            options.TermsOfServiceUrl = swaggerOptions.TermsOfServiceUrl;
            options.ContactName = swaggerOptions.ContactName;
            options.ContactEmail = swaggerOptions.ContactEmail;
            options.ContactUrl = swaggerOptions.ContactUrl;
            options.LicenseName = swaggerOptions.LicenseName;
            options.LicenseUrl = swaggerOptions.LicenseUrl;
            options.IncludeXmlComments = swaggerOptions.IncludeXmlComments;
            options.RoutePrefix = swaggerOptions.RoutePrefix;
            options.EnableAnnotations = swaggerOptions.EnableAnnotations;
            options.CustomCss = swaggerOptions.CustomCss;
            options.DocumentTitle = swaggerOptions.DocumentTitle;
            options.EnableDeepLinking = swaggerOptions.EnableDeepLinking;
            options.DisplayOperationId = swaggerOptions.DisplayOperationId;
            options.DisplayRequestDuration = swaggerOptions.DisplayRequestDuration;
            options.DefaultModelsExpandDepth = swaggerOptions.DefaultModelsExpandDepth;
            options.DefaultModelExpandDepth = swaggerOptions.DefaultModelExpandDepth;
            options.ShowExtensions = swaggerOptions.ShowExtensions;
            options.ShowCommonExtensions = swaggerOptions.ShowCommonExtensions;
        });
    }

    /// <summary>
    /// Initializes the Swashbuckle module.
    /// </summary>
    /// <param name="context">The module context.</param>
    public override void Initialize(ModuleContext context)
    {
        var logger = context.GetService<ILogger<EssentialsSwashbuckleModule>>();
        logger?.LogInformation("Swashbuckle module initialized successfully. Swagger documentation is configured.");
    }
}