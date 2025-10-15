using HMS.Essentials.Modularity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace HMS.Essentials.Swashbuckle.Tests.Helpers;

/// <summary>
/// Helper methods for creating test contexts and configurations.
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Creates a module context for testing.
    /// </summary>
    public static ModuleContext CreateModuleContext()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        return new ModuleContext(services, configuration);
    }

    /// <summary>
    /// Creates a module context with custom service collection.
    /// </summary>
    public static ModuleContext CreateModuleContext(IServiceCollection services)
    {
        var configuration = CreateConfiguration();
        return new ModuleContext(services, configuration);
    }

    /// <summary>
    /// Creates a module context with custom configuration.
    /// </summary>
    public static ModuleContext CreateModuleContext(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        return new ModuleContext(services, configuration);
    }

    /// <summary>
    /// Creates a module context with custom services and configuration.
    /// </summary>
    public static ModuleContext CreateModuleContext(IServiceCollection services, IConfiguration configuration)
    {
        return new ModuleContext(services, configuration);
    }

    /// <summary>
    /// Creates a default configuration builder.
    /// </summary>
    public static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
    }

    /// <summary>
    /// Creates a configuration with Swagger settings.
    /// </summary>
    public static IConfiguration CreateSwaggerConfiguration(Dictionary<string, string?> settings)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    /// <summary>
    /// Creates a fake IWebHostEnvironment for testing.
    /// </summary>
    public static IWebHostEnvironment CreateFakeWebHostEnvironment()
    {
        return new FakeWebHostEnvironment();
    }

    /// <summary>
    /// Fake implementation of IWebHostEnvironment for testing.
    /// </summary>
    private class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public string EnvironmentName { get; set; } = "Development";
    }
}
