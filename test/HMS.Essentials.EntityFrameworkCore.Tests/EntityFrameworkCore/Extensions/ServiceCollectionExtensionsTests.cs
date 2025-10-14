using FluentAssertions;
using HMS.Essentials.EntityFrameworkCore.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HMS.Essentials.EntityFrameworkCore.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEfCoreServices_WithoutConfiguration_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEfCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var connectionStringProvider = serviceProvider.GetService<IConnectionStringProvider>();
        connectionStringProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddEfCoreServices_WithConfiguration_ShouldRegisterServicesWithOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultConnectionString = "Server=localhost;Database=TestDb;";

        // Act
        services.AddEfCoreServices(options =>
        {
            options.DefaultConnectionString = defaultConnectionString;
            options.EnableDetailedErrors = true;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<EfCoreOptions>>();
        options.Should().NotBeNull();
        options!.Value.DefaultConnectionString.Should().Be(defaultConnectionString);
        options.Value.EnableDetailedErrors.Should().BeTrue();
    }

    [Fact]
    public void AddEfCoreServices_ShouldRegisterConnectionStringProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEfCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IConnectionStringProvider>();
        provider.Should().NotBeNull();
        provider.Should().BeOfType<DefaultConnectionStringProvider>();
    }

    [Fact]
    public void AddEfCoreServices_WithConnectionStrings_ShouldConfigureProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEfCoreServices(options =>
        {
            options.DefaultConnectionString = "DefaultConnection";
            options.ConnectionStrings["Context1"] = "ConnectionString1";
            options.ConnectionStrings["Context2"] = "ConnectionString2";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IConnectionStringProvider>();
        provider.Should().NotBeNull();
        provider!.GetConnectionString().Should().Be("DefaultConnection");
        provider.GetConnectionString("Context1").Should().Be("ConnectionString1");
        provider.GetConnectionString("Context2").Should().Be("ConnectionString2");
    }

    [Fact]
    public void AddEfCoreServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddEfCoreServices();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddEfCoreServices_WithAllOptions_ShouldConfigureCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEfCoreServices(options =>
        {
            options.DefaultConnectionString = "Server=localhost;Database=TestDb;";
            options.EnableDetailedErrors = true;
            options.EnableSensitiveDataLogging = true;
            options.MaxRetryCount = 10;
            options.MaxRetryDelay = 60;
            options.UseLazyLoadingProxies = true;
            options.CommandTimeout = 120;
            options.AutoMigrateOnStartup = true;
            options.SeedDataOnStartup = true;
            options.ConnectionStrings["TestContext"] = "TestConnectionString";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var efCoreOptions = serviceProvider.GetService<IOptions<EfCoreOptions>>();
        efCoreOptions.Should().NotBeNull();
        
        var optionsValue = efCoreOptions!.Value;
        optionsValue.DefaultConnectionString.Should().Be("Server=localhost;Database=TestDb;");
        optionsValue.EnableDetailedErrors.Should().BeTrue();
        optionsValue.EnableSensitiveDataLogging.Should().BeTrue();
        optionsValue.MaxRetryCount.Should().Be(10);
        optionsValue.MaxRetryDelay.Should().Be(60);
        optionsValue.UseLazyLoadingProxies.Should().BeTrue();
        optionsValue.CommandTimeout.Should().Be(120);
        optionsValue.AutoMigrateOnStartup.Should().BeTrue();
        optionsValue.SeedDataOnStartup.Should().BeTrue();
        optionsValue.ConnectionStrings.Should().ContainKey("TestContext");
    }

    [Fact]
    public void AddEfCoreServices_MultipleCalls_ShouldNotDuplicate()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEfCoreServices();
        services.AddEfCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var providers = serviceProvider.GetServices<IConnectionStringProvider>().ToList();
        providers.Should().HaveCount(1);
    }

    [Fact]
    public void AddConnectionString_ShouldAddConnectionStringToProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEfCoreServices(options =>
        {
            options.DefaultConnectionString = "DefaultConnection";
        });

        // Act
        services.AddConnectionString("CustomContext", "CustomConnectionString");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IConnectionStringProvider>();
        provider.Should().NotBeNull();
        provider!.GetConnectionString("CustomContext").Should().Be("CustomConnectionString");
    }

    [Fact]
    public void AddConnectionString_WithoutEfCoreServices_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddConnectionString("Context", "ConnectionString");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AddConnectionString_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEfCoreServices();

        // Act
        var result = services.AddConnectionString("Context", "ConnectionString");

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddConnectionString_Multiple_ShouldAddAll()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEfCoreServices();

        // Act
        services.AddConnectionString("Context1", "ConnectionString1");
        services.AddConnectionString("Context2", "ConnectionString2");
        services.AddConnectionString("Context3", "ConnectionString3");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IConnectionStringProvider>();
        provider!.GetConnectionString("Context1").Should().Be("ConnectionString1");
        provider.GetConnectionString("Context2").Should().Be("ConnectionString2");
        provider.GetConnectionString("Context3").Should().Be("ConnectionString3");
    }

    [Fact]
    public void AddEfCoreServices_WithEmptyConfiguration_ShouldUseDefaults()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEfCoreServices(options => { });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var efCoreOptions = serviceProvider.GetService<IOptions<EfCoreOptions>>();
        efCoreOptions.Should().NotBeNull();
        
        var optionsValue = efCoreOptions!.Value;
        optionsValue.EnableDetailedErrors.Should().BeFalse();
        optionsValue.MaxRetryCount.Should().Be(6);
        optionsValue.MaxRetryDelay.Should().Be(30);
    }
}
