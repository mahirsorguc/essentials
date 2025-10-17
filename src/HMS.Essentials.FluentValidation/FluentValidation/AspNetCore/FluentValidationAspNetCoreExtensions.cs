using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.FluentValidation.AspNetCore;

/// <summary>
/// Extension methods for configuring FluentValidation with ASP.NET Core.
/// </summary>
public static class FluentValidationAspNetCoreExtensions
{
    /// <summary>
    /// Adds FluentValidation to ASP.NET Core with automatic model validation.
    /// This enables automatic validation of DTOs in Web API controllers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="disableDataAnnotations">Whether to disable DataAnnotations validation.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFluentValidationForAspNetCore(
        this IServiceCollection services,
        bool disableDataAnnotations = true)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Add FluentValidation to ASP.NET Core
        services.AddFluentValidationAutoValidation(config =>
        {
            config.DisableDataAnnotationsValidation = disableDataAnnotations;
        });

        return services;
    }

    /// <summary>
    /// Configures API behavior options to customize validation error responses.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for customizing validation responses.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection ConfigureValidationErrorResponses(
        this IServiceCollection services,
        Action<ApiBehaviorOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            // Keep the default model state validation behavior
            // but allow customization
            configure?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// Configures API behavior to use custom validation error response format.
    /// Returns a standardized error response with validation errors grouped by property.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="includeErrorDetails">Whether to include detailed error information.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection UseStandardValidationErrorResponse(
        this IServiceCollection services,
        bool includeErrorDetails = true)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = new ValidationErrorResponse
                {
                    Type = "ValidationError",
                    Title = "One or more validation errors occurred.",
                    Status = 400,
                    Errors = errors
                };

                if (includeErrorDetails)
                {
                    response.TraceId = context.HttpContext.TraceIdentifier;
                    response.Instance = context.HttpContext.Request.Path;
                }

                return new BadRequestObjectResult(response)
                {
                    StatusCode = 400
                };
            };
        });

        return services;
    }

    /// <summary>
    /// Configures client-side validation integration.
    /// Enables client-side validation with FluentValidation rules.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFluentValidationClientSide(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddFluentValidationClientsideAdapters();

        return services;
    }
}

/// <summary>
/// Standardized validation error response model.
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// Gets or sets the error type.
    /// </summary>
    public string Type { get; set; } = "ValidationError";

    /// <summary>
    /// Gets or sets the error title.
    /// </summary>
    public string Title { get; set; } = "Validation failed";

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int Status { get; set; } = 400;

    /// <summary>
    /// Gets or sets the validation errors grouped by property name.
    /// </summary>
    public Dictionary<string, string[]> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the trace identifier for debugging.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the request path.
    /// </summary>
    public string? Instance { get; set; }

    /// <summary>
    /// Gets or sets additional error details.
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }
}
