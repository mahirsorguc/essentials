using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.UnitOfWork.Tests;

/// <summary>
/// Tests for EssentialsUnitOfWorkModule.
/// </summary>
public class EssentialsUnitOfWorkModuleTests
{
    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder().Build();
    }

    [Fact]
    public void Module_ShouldHaveCorrectDependencies()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsUnitOfWorkModule);
        var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .FirstOrDefault();

        // Assert
        dependsOnAttribute.ShouldNotBeNull();
        dependsOnAttribute.DependsOn.ShouldContain(typeof(EssentialsCoreModule));
    }

    [Fact]
    public void Module_CanBeInstantiated()
    {
        // Act & Assert
        Should.NotThrow(() => new EssentialsUnitOfWorkModule());
    }

    [Fact]
    public void Module_InheritsFromEssentialsModule()
    {
        // Arrange
        var module = new EssentialsUnitOfWorkModule();

        // Assert
        module.ShouldBeAssignableTo<EssentialsModule>();
    }

    [Fact]
    public void ConfigureServices_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsUnitOfWorkModule();

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }

    [Fact]
    public void Initialize_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsUnitOfWorkModule();

        // Act & Assert
        Should.NotThrow(() => module.Initialize(context));
    }

    [Fact]
    public void Shutdown_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsUnitOfWorkModule();

        // Act & Assert
        Should.NotThrow(() => module.Shutdown(context));
    }

    [Fact]
    public void Module_IsPublicClass()
    {
        // Arrange
        var moduleType = typeof(EssentialsUnitOfWorkModule);

        // Assert
        moduleType.IsPublic.ShouldBeTrue();
        moduleType.IsClass.ShouldBeTrue();
    }

    [Fact]
    public void Module_IsNotAbstract()
    {
        // Arrange
        var moduleType = typeof(EssentialsUnitOfWorkModule);

        // Assert
        moduleType.IsAbstract.ShouldBeFalse();
    }

    [Fact]
    public void Module_HasParameterlessConstructor()
    {
        // Arrange
        var moduleType = typeof(EssentialsUnitOfWorkModule);

        // Act
        var constructor = moduleType.GetConstructor(Type.EmptyTypes);

        // Assert
        constructor.ShouldNotBeNull();
    }

    [Fact]
    public void Module_DependsOnAttribute_HasCorrectProperties()
    {
        // Arrange
        var moduleType = typeof(EssentialsUnitOfWorkModule);
        var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .FirstOrDefault();

        // Assert
        dependsOnAttribute.ShouldNotBeNull();
        dependsOnAttribute.DependsOn.ShouldNotBeNull();
        dependsOnAttribute.DependsOn.Length.ShouldBe(1);
        dependsOnAttribute.DependsOn[0].ShouldBe(typeof(EssentialsCoreModule));
    }
}
