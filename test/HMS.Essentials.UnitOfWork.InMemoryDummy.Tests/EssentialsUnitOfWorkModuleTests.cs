using FluentAssertions;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.UnitOfWork.InMemoryDummy;

public class EssentialsUnitOfWorkModuleTests
{
    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder().Build();
    }

    [Fact]
    public void ConfigureServices_RegistersUnitOfWork()
    {
        // Arrange
        var services = new ServiceCollection();
        var context = new ModuleContext(services, CreateConfiguration());
        var module = new EssentialsUnitOfWorkModule();

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        unitOfWork.Should().NotBeNull();
        unitOfWork.Should().BeOfType<InMemoryUnitOfWork>();
    }

    [Fact]
    public void ConfigureServices_RegistersAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var context = new ModuleContext(services, CreateConfiguration());
        var module = new EssentialsUnitOfWorkModule();

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var uow1 = scope1.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var uow2 = scope2.ServiceProvider.GetRequiredService<IUnitOfWork>();

        uow1.Should().NotBeSameAs(uow2);
    }

    [Fact]
    public void Initialize_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var context = new ModuleContext(services, CreateConfiguration());
        var module = new EssentialsUnitOfWorkModule();

        // Act
        var act = () => module.Initialize(context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Shutdown_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var context = new ModuleContext(services, CreateConfiguration());
        var module = new EssentialsUnitOfWorkModule();

        // Act
        var act = () => module.Shutdown(context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Module_HasCorrectDependencies()
    {
        // Arrange
        var moduleType = typeof(EssentialsUnitOfWorkModule);

        // Act
        var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

        // Assert
        dependsOnAttributes.Should().NotBeEmpty();
        var attribute = dependsOnAttributes[0] as DependsOnAttribute;
        attribute!.DependsOn.Should().Contain(typeof(EssentialsCoreModule));
    }
}
