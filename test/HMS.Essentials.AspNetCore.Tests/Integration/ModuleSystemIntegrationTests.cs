using HMS.Essentials.AspNetCore.TestModules;
using HMS.Essentials.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.AspNetCore.Integration;

/// <summary>
/// Integration tests for module system with actual ASP.NET Core application.
/// </summary>
public class ModuleSystemIntegrationTests : IDisposable
{
    private WebApplication? _app;

    [Fact]
    public async Task ModuleSystem_ShouldWorkWithMinimalApi()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<TestModule>();

        _app = builder.Build();

        _app.MapGet("/test", (ITestService service) => service.GetMessage());

        // Act
        await _app.StartAsync();
        
        using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        
        // Assert
        _app.Services.GetRequiredService<ITestService>().Should().NotBeNull();
        
        await _app.StopAsync();
    }

    [Fact]
    public async Task ModuleSystem_ShouldWorkWithDependencyInjection()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<RootTestModule>();

        _app = builder.Build();

        // Act
        await _app.StartAsync();

        using var scope = _app.Services.CreateScope();
        var rootService = scope.ServiceProvider.GetRequiredService<IRootService>();
        var dependentService = scope.ServiceProvider.GetRequiredService<IDependentService>();
        var testService = scope.ServiceProvider.GetRequiredService<ITestService>();

        // Assert
        rootService.Should().NotBeNull();
        dependentService.Should().NotBeNull();
        testService.Should().NotBeNull();

        await _app.StopAsync();
    }

    [Fact]
    public void ModuleSystem_ShouldProvideAccessToAllModuleInformation()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<RootTestModule>();

        _app = builder.Build();

        // Act
        var appHost = _app.GetApplicationHost();

        // Assert
        appHost.Modules.Should().NotBeEmpty();
        appHost.Modules.Should().Contain(m => m.ModuleType == typeof(RootTestModule));
        appHost.Modules.Should().Contain(m => m.ModuleType == typeof(DependentTestModule));
        appHost.Modules.Should().Contain(m => m.ModuleType == typeof(TestModule));
        
        foreach (var module in appHost.Modules)
        {
            module.State.Should().Be(ModuleState.Initialized);
            module.Name.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void ModuleSystem_ShouldMaintainModuleHierarchy()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<RootTestModule>();

        _app = builder.Build();

        // Act
        var appHost = _app.GetApplicationHost();

        // Assert
        var rootModule = appHost.Modules.FirstOrDefault(m => m.ModuleType == typeof(RootTestModule));
        rootModule.Should().NotBeNull();
    }

    [Fact]
    public void ModuleSystem_WithCustomConfiguration_ShouldApplyConfiguration()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        var customServiceRegistered = false;

        builder.AddModuleSystem<TestModule>(appBuilder =>
        {
            appBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ICustomService, CustomService>();
                customServiceRegistered = true;
            });
        });

        _app = builder.Build();

        // Act
        var customService = _app.Services.GetService<ICustomService>();

        // Assert
        customServiceRegistered.Should().BeTrue();
        customService.Should().NotBeNull();
        customService.Should().BeOfType<CustomService>();
    }

    [Fact]
    public async Task ModuleSystem_ShouldSupportApplicationLifecycle()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<TestModule>();

        _app = builder.Build();

        // Act
        await _app.StartAsync();
        
        // Assert - Application should be running
        var appHost = _app.GetApplicationHost();
        appHost.Should().NotBeNull();
        appHost.Modules.Should().AllSatisfy(m => 
            m.State.Should().Be(ModuleState.Initialized));

        // Cleanup
        await _app.StopAsync();
    }

    [Fact]
    public void ModuleSystem_ShouldAllowMultipleServiceScopes()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<RootTestModule>();

        _app = builder.Build();

        // Act
        using var scope1 = _app.Services.CreateScope();
        using var scope2 = _app.Services.CreateScope();

        var service1 = scope1.ServiceProvider.GetRequiredService<IRootService>();
        var service2 = scope2.ServiceProvider.GetRequiredService<IRootService>();

        // Assert - Singleton services should be the same instance
        service1.Should().BeSameAs(service2);
    }

    [Fact]
    public void ModuleSystem_ApplicationHost_ShouldBeAccessibleFromDI()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.AddModuleSystem<TestModule>();

        _app = builder.Build();

        // Act
        var appHost1 = _app.Services.GetRequiredService<ApplicationHost>();
        var appHost2 = _app.Services.GetRequiredService<ApplicationHost>();

        // Assert
        appHost1.Should().NotBeNull();
        appHost1.Should().BeSameAs(appHost2);
    }

    public void Dispose()
    {
        _app?.DisposeAsync().AsTask().Wait();
    }
}

public interface ICustomService
{
    string GetName();
}

public class CustomService : ICustomService
{
    public string GetName() => "Custom Service";
}
