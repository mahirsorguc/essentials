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
            .UseRootModule<EssentialsCoreTestsModule>()
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
            .UseRootModule<EssentialsCoreTestsModule>()
            .Build();

        var service = host.Services.GetService<TestServicePlaceholder>();

        // Assert
        service.ShouldNotBeNull();
    }
}