using HMS.Essential.Logging;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace HMS.Essentials.Data;

public class EssentialsDataModuleTests
{
    [Fact]
    public void Module_Should_Depend_On_LoggingModule()
    {
        // Arrange
        var moduleType = typeof(EssentialsDataModule);

        // Act
        var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .ToArray();

        // Assert
        dependsOnAttributes.ShouldNotBeEmpty();
        dependsOnAttributes.Any(attr => attr.DependsOn.Contains(typeof(LoggingEssentialsModule)))
            .ShouldBeTrue();
    }

    [Fact]
    public void ConfigureServices_Should_Register_IDataRepository()
    {
        // Arrange
        var module = new EssentialsDataModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var repository = serviceProvider.GetService<IDataRepository>();
        repository.ShouldNotBeNull();
        repository.ShouldBeOfType<DataRepository>();
    }

    [Fact]
    public void ConfigureServices_Should_Register_IDataRepository_As_Singleton()
    {
        // Arrange
        var module = new EssentialsDataModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var repository1 = serviceProvider.GetService<IDataRepository>();
        var repository2 = serviceProvider.GetService<IDataRepository>();
        repository1.ShouldBe(repository2); // Should be same instance
    }

    [Fact]
    public void OnInitialize_Should_Complete_Successfully()
    {
        // Arrange
        var module = new EssentialsDataModule();
        var services = new ServiceCollection();
        
        var mockLogService = new Mock<ILogService>();
        mockLogService.Setup(l => l.LogInfo(It.IsAny<string>()));
        services.AddSingleton(mockLogService.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        var context = TestHelper.CreateModuleContext(services);
        
        // Set ServiceProvider using reflection since it's internal
        var serviceProviderProperty = typeof(ModuleContext).GetProperty("ServiceProvider");
        serviceProviderProperty?.SetValue(context, serviceProvider);

        // Act & Assert - Should not throw
        module.Initialize(context);
    }

    [Fact]
    public void Module_Should_Be_Instantiable()
    {
        // Act
        var module = new EssentialsDataModule();

        // Assert
        module.ShouldNotBeNull();
        module.ShouldBeAssignableTo<EssentialsModule>();
    }

    [Fact]
    public void Module_Should_Implement_IModule()
    {
        // Arrange
        var module = new EssentialsDataModule();

        // Assert
        module.ShouldBeAssignableTo<IModule>();
    }

    [Fact]
    public void ConfigureServices_Should_Not_Throw()
    {
        // Arrange
        var module = new EssentialsDataModule();
        var context = TestHelper.CreateModuleContext();

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }
}
