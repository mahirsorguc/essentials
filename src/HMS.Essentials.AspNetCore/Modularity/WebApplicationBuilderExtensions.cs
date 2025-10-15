using HMS.Essentials.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.AspNetCore.Modularity;

/// <summary>
/// Extension methods for integrating the HMS Essentials module system with ASP.NET Core WebApplicationBuilder.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the HMS Essentials module system to the WebApplicationBuilder.
    /// </summary>
    /// <typeparam name="TRootModule">The root module type that defines the application's module hierarchy.</typeparam>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <returns>The WebApplicationBuilder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.AddModuleSystem&lt;MainAppWebApiModule&gt;();
    /// var app = builder.Build();
    /// </code>
    /// </example>
    public static WebApplicationBuilder AddModuleSystem<TRootModule>(this WebApplicationBuilder builder)
        where TRootModule : IModule, new()
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddModuleSystem<TRootModule>(null);
    }

    /// <summary>
    /// Adds the HMS Essentials module system to the WebApplicationBuilder with additional configuration.
    /// </summary>
    /// <typeparam name="TRootModule">The root module type that defines the application's module hierarchy.</typeparam>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <param name="configure">Optional action to configure the ApplicationBuilder before building.</param>
    /// <returns>The WebApplicationBuilder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.AddModuleSystem&lt;MainAppWebApiModule&gt;(appBuilder =>
    /// {
    ///     appBuilder.ConfigureServices(services =>
    ///     {
    ///         services.AddSingleton&lt;ICustomService, CustomService&gt;();
    ///     });
    /// });
    /// var app = builder.Build();
    /// </code>
    /// </example>
    public static WebApplicationBuilder AddModuleSystem<TRootModule>(
        this WebApplicationBuilder builder,
        Action<HMS.Essentials.Modularity.ApplicationBuilder>? configure)
        where TRootModule : IModule, new()
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Create the module system's ApplicationBuilder
        var appBuilder = new HMS.Essentials.Modularity.ApplicationBuilder(builder.Services, builder.Configuration)
            .WithEnvironment(builder.Environment.EnvironmentName)
            .UseRootModule<TRootModule>();

        // Allow additional configuration
        configure?.Invoke(appBuilder);

        // Build the ApplicationHost and store it for later retrieval
        var applicationHost = appBuilder.Build();

        // Store the ApplicationHost in the service collection as a singleton
        builder.Services.AddSingleton(applicationHost);

        return builder;
    }

    /// <summary>
    /// Gets the ApplicationHost from the built WebApplication.
    /// This allows access to module information and services managed by the module system.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The ApplicationHost instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when app is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the module system has not been configured.</exception>
    /// <example>
    /// <code>
    /// var app = builder.Build();
    /// var appHost = app.GetApplicationHost();
    /// 
    /// foreach (var module in appHost.Modules)
    /// {
    ///     Console.WriteLine($"Module: {module.Name}");
    /// }
    /// </code>
    /// </example>
    public static ApplicationHost GetApplicationHost(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var applicationHost = app.Services.GetService<ApplicationHost>();
        if (applicationHost == null)
        {
            throw new InvalidOperationException(
                "ApplicationHost not found. Make sure to call AddModuleSystem<TRootModule>() on WebApplicationBuilder before building.");
        }

        return applicationHost;
    }
}
