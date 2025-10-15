using HMS.Essentials.Swashbuckle.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HMS.Essentials.Swashbuckle.Extensions;

/// <summary>
/// Extension methods for configuring Swagger middleware.
/// </summary>
public static class SwaggerMiddlewareExtensions
{
    /// <summary>
    /// Adds Swagger middleware to the application pipeline.
    /// Only enables Swagger UI in Development environment by default.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = app.ApplicationServices.GetService<SwaggerOptions>() ?? new SwaggerOptions();
        var env = app.ApplicationServices.GetService<IHostEnvironment>();

        // Check if Swagger should be enabled based on environment
        var shouldEnable = options.EnableSwaggerUI &&
                          (env?.IsDevelopment() == true || options.EnableInProduction);

        if (!shouldEnable)
        {
            return app;
        }

        // Enable middleware to serve generated Swagger as a JSON endpoint
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", $"{options.Title} {options.Version}");
            c.RoutePrefix = options.RoutePrefix;

            // Configure UI options
            if (!string.IsNullOrWhiteSpace(options.DocumentTitle))
            {
                c.DocumentTitle = options.DocumentTitle;
            }

            if (!string.IsNullOrWhiteSpace(options.CustomCss))
            {
                c.InjectStylesheet(options.CustomCss);
            }

            // Enable deep linking
            c.EnableDeepLinking();

            // Display options
            if (options.DisplayOperationId)
            {
                c.DisplayOperationId();
            }

            if (options.DisplayRequestDuration)
            {
                c.DisplayRequestDuration();
            }

            // Models display depth
            c.DefaultModelsExpandDepth(options.DefaultModelsExpandDepth);
            c.DefaultModelExpandDepth(options.DefaultModelExpandDepth);

            // Show extensions
            if (options.ShowExtensions)
            {
                c.ShowExtensions();
            }

            if (options.ShowCommonExtensions)
            {
                c.ShowCommonExtensions();
            }
        });

        return app;
    }

    /// <summary>
    /// Adds Swagger middleware to the application pipeline with custom configuration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Action to configure Swagger options.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSwaggerDocumentation(
        this IApplicationBuilder app,
        Action<SwaggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new SwaggerOptions();
        configure(options);

        // Replace the registered options
        var serviceProvider = app.ApplicationServices;
        
        return app.UseSwaggerDocumentation();
    }
}
