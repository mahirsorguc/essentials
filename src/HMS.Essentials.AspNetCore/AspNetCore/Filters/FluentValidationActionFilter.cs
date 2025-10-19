using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.AspNetCore.Filters;

/// <summary>
/// Custom action filter that automatically validates request models using FluentValidation.
/// This filter intercepts requests and validates the action parameters using registered validators.
/// </summary>
public class FluentValidationActionFilter : IAsyncActionFilter
{
    private readonly ILogger<FluentValidationActionFilter> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentValidationActionFilter"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic information.</param>
    public FluentValidationActionFilter(ILogger<FluentValidationActionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the filter asynchronously, validating action parameters before the action executes.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The next action filter in the pipeline.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Skip validation if ModelState is already invalid
        if (!context.ModelState.IsValid)
        {
            _logger.LogDebug("ModelState is already invalid, skipping FluentValidation");
            await next();
            return;
        }

        // Validate each action parameter
        var hasValidationErrors = false;

        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            // Get the parameter value
            if (!context.ActionArguments.TryGetValue(parameter.Name, out var parameterValue))
            {
                continue;
            }

            // Skip null values
            if (parameterValue == null)
            {
                continue;
            }

            // Get the validator type
            var parameterType = parameterValue.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);

            // Try to get the validator from DI
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator == null)
            {
                // No validator registered for this type, skip
                _logger.LogTrace("No validator found for type {ParameterType}", parameterType.Name);
                continue;
            }

            // Perform validation
            _logger.LogDebug("Validating parameter {ParameterName} of type {ParameterType}", 
                parameter.Name, parameterType.Name);

            var validationContext = new ValidationContext<object>(parameterValue);
            var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                hasValidationErrors = true;

                _logger.LogWarning("Validation failed for parameter {ParameterName}. Errors: {ErrorCount}", 
                    parameter.Name, validationResult.Errors.Count);

                // Add validation errors to ModelState
                foreach (var error in validationResult.Errors)
                {
                    var propertyName = string.IsNullOrEmpty(error.PropertyName) 
                        ? parameter.Name 
                        : $"{parameter.Name}.{error.PropertyName}";

                    context.ModelState.AddModelError(propertyName, error.ErrorMessage);
                }
            }
            else
            {
                _logger.LogDebug("Validation passed for parameter {ParameterName}", parameter.Name);
            }
        }

        // If validation failed, return BadRequest with validation errors
        if (hasValidationErrors)
        {
            _logger.LogInformation("Request validation failed, returning BadRequest");

            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            context.Result = new BadRequestObjectResult(new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = errors,
                TraceId = context.HttpContext.TraceIdentifier
            });

            return;
        }

        // Continue to the next filter or action
        await next();
    }
}
