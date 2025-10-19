using HMS.Essentials.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.AspNetCore.Extensions;

/// <summary>
/// Extension methods for configuring FluentValidation action filters.
/// </summary>
public static class FluentValidationFilterExtensions
{
    /// <summary>
    /// Adds FluentValidation action filter globally to all controllers.
    /// This will automatically validate all action parameters using registered FluentValidation validators.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddControllers()
    ///     .AddFluentValidationAutoValidation();
    /// </code>
    /// </example>
    public static IMvcBuilder AddFluentValidationAutoValidation(this IMvcBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<FluentValidationActionFilter>();
        });

        return builder;
    }

    /// <summary>
    /// Adds FluentValidation action filter globally to all controllers.
    /// This will automatically validate all action parameters using registered FluentValidation validators.
    /// </summary>
    /// <param name="options">The MVC options.</param>
    /// <returns>The MVC options for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddControllers(options =>
    /// {
    ///     options.AddFluentValidationAutoValidation();
    /// });
    /// </code>
    /// </example>
    public static MvcOptions AddFluentValidationAutoValidation(this MvcOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Filters.Add<FluentValidationActionFilter>();

        return options;
    }

    /// <summary>
    /// Adds FluentValidation action filter with custom order.
    /// This will automatically validate all action parameters using registered FluentValidation validators.
    /// </summary>
    /// <param name="builder">The MVC builder.</param>
    /// <param name="order">The order in which the filter should execute.</param>
    /// <returns>The MVC builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddControllers()
    ///     .AddFluentValidationAutoValidation(order: 10);
    /// </code>
    /// </example>
    public static IMvcBuilder AddFluentValidationAutoValidation(this IMvcBuilder builder, int order)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<FluentValidationActionFilter>(order);
        });

        return builder;
    }

    /// <summary>
    /// Adds FluentValidation action filter to MVC options with custom order.
    /// This will automatically validate all action parameters using registered FluentValidation validators.
    /// </summary>
    /// <param name="options">The MVC options.</param>
    /// <param name="order">The order in which the filter should execute.</param>
    /// <returns>The MVC options for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddControllers(options =>
    /// {
    ///     options.AddFluentValidationAutoValidation(order: 10);
    /// });
    /// </code>
    /// </example>
    public static MvcOptions AddFluentValidationAutoValidation(this MvcOptions options, int order)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Filters.Add<FluentValidationActionFilter>(order);

        return options;
    }
}
