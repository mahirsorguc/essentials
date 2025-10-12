using HMS.Essential.Logging;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace HMS.Essentials.Logging;

public class LoggingEssentialsModuleTests
{
    [Fact]
    public void Module_Should_Depend_On_CoreModule()
    {
        // Arrange
        var moduleType = typeof(LoggingEssentialsModule);

        // Act
        var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .ToArray();

        // Assert
        dependsOnAttributes.ShouldNotBeEmpty();
        dependsOnAttributes.Any(attr => attr.DependsOn.Contains(typeof(EssentialsCoreModule)))
            .ShouldBeTrue();
    }

    [Fact]
    public void ConfigureServices_Should_Register_Logging()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        loggerFactory.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_Should_Register_ILogService()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var logService = serviceProvider.GetService<ILogService>();
        logService.ShouldNotBeNull();
        logService.ShouldBeOfType<LogService>();
    }

    [Fact]
    public void ConfigureServices_Should_Register_ILogService_As_Singleton()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var logService1 = serviceProvider.GetService<ILogService>();
        var logService2 = serviceProvider.GetService<ILogService>();
        logService1.ShouldBe(logService2); // Should be same instance
    }

    [Fact]
    public void ConfigureServices_Should_Configure_Console_Logging()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var logger = serviceProvider.GetService<ILogger<LoggingEssentialsModuleTests>>();
        logger.ShouldNotBeNull();
    }

    [Fact]
    public void OnInitialize_Should_Use_Logger()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        
        var context = TestHelper.CreateModuleContext(services);
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        Should.NotThrow(() => module.Initialize(context));
    }

    [Fact]
    public void OnShutdown_Should_Not_Throw()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var context = TestHelper.CreateModuleContext();

        // Act & Assert
        Should.NotThrow(() => module.Shutdown(context));
    }

    [Fact]
    public void Module_Should_Be_Instantiable()
    {
        // Act
        var module = new LoggingEssentialsModule();

        // Assert
        module.ShouldNotBeNull();
        module.ShouldBeAssignableTo<EssentialsModule>();
    }

    [Fact]
    public void Module_Should_Implement_IModule()
    {
        // Arrange
        var module = new LoggingEssentialsModule();

        // Assert
        module.ShouldBeAssignableTo<IModule>();
    }

    [Fact]
    public void ConfigureServices_Should_Not_Throw()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var context = TestHelper.CreateModuleContext();

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }

    [Fact]
    public void Logger_Should_Be_Able_To_Log_After_Configuration()
    {
        // Arrange
        var module = new LoggingEssentialsModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var logger = serviceProvider.GetRequiredService<ILogger<LoggingEssentialsModuleTests>>();

        // Assert
        logger.ShouldNotBeNull();
        Should.NotThrow(() => logger.LogInformation("Test log message"));
    }
}
