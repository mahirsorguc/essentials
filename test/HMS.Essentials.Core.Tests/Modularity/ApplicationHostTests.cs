using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.Modularity;

public class ApplicationHostTests
{
    [Fact]
    public void Build_ShouldCreateApplicationHost()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var host = builder
            .AddModule<TestModule>()
            .Build();

        // Assert
        host.ShouldNotBeNull();
        host.Services.ShouldNotBeNull();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveRegisteredServices()
    {
        // Arrange & Act
        var host = ApplicationBuilder.Create()
            .AddModule<TestModule>()
            .Build();

        var service = host.Services.GetService<TestService>();

        // Assert
        service.ShouldNotBeNull();
    }

    private class TestModule : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            context.Services.AddSingleton<TestService>();
        }
    }

    private class TestService
    {
    }
}
