using HMS.Essentials.AspNetCore;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HMS.Essentials.AspNetCore.Tests.Modularity;

public class EssentialsAspNetCoreModuleTests
{
    [Fact]
    public void EssentialsAspNetCoreModule_ShouldInheritFromEssentialsModule()
    {
        // Arrange & Act
        var module = new EssentialsAspNetCoreModule();

        // Assert
        Assert.IsAssignableFrom<EssentialsModule>(module);
        Assert.IsAssignableFrom<IModule>(module);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldHaveDependsOnAttribute()
    {
        // Arrange & Act
        var dependsOnAttributes = typeof(EssentialsAspNetCoreModule)
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .ToList();

        // Assert
        Assert.Single(dependsOnAttributes);
        Assert.Contains(typeof(EssentialsCoreModule), dependsOnAttributes[0].DependsOn);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldDependOnEssentialsCoreModule()
    {
        // Arrange
        var dependsOnAttribute = typeof(EssentialsAspNetCoreModule)
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .FirstOrDefault();

        // Act & Assert
        Assert.NotNull(dependsOnAttribute);
        Assert.Contains(typeof(EssentialsCoreModule), dependsOnAttribute.DependsOn);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_CanBeInstantiated()
    {
        // Arrange & Act
        var exception = Record.Exception(() => new EssentialsAspNetCoreModule());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldBeUsableAsRootModule()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var appBuilder = new ApplicationBuilder(services, configuration);

        // Act
        var exception = Record.Exception(() => 
            appBuilder.UseRootModule<EssentialsAspNetCoreModule>().Build());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldLoadEssentialsCoreModuleFirst()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var appBuilder = new ApplicationBuilder(services, configuration);

        // Act
        var applicationHost = appBuilder.UseRootModule<EssentialsAspNetCoreModule>().Build();

        // Assert
        Assert.NotNull(applicationHost);
        Assert.NotEmpty(applicationHost.Modules);
        
        // EssentialsCoreModule should be loaded before EssentialsAspNetCoreModule
        var coreModuleIndex = -1;
        var aspNetCoreModuleIndex = -1;

        for (int i = 0; i < applicationHost.Modules.Count; i++)
        {
            if (applicationHost.Modules[i].ModuleType == typeof(EssentialsCoreModule))
                coreModuleIndex = i;
            if (applicationHost.Modules[i].ModuleType == typeof(EssentialsAspNetCoreModule))
                aspNetCoreModuleIndex = i;
        }

        Assert.True(coreModuleIndex >= 0, "EssentialsCoreModule not found");
        Assert.True(aspNetCoreModuleIndex >= 0, "EssentialsAspNetCoreModule not found");
        Assert.True(coreModuleIndex < aspNetCoreModuleIndex, 
            "EssentialsCoreModule should be loaded before EssentialsAspNetCoreModule");
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ConfigureServices_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreModule();

        // Act
        var exception = Record.Exception(() => module.ConfigureServices(context));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_WithServiceProvider_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var appBuilder = new ApplicationBuilder(services, configuration);

        // Act
        var exception = Record.Exception(() =>
        {
            var applicationHost = appBuilder.UseRootModule<EssentialsAspNetCoreModule>().Build();
            var serviceProvider = applicationHost.Services;
            // Try to get any service to verify module loaded correctly
            var _ = serviceProvider.GetService<IConfiguration>();
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldHaveCorrectNamespace()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAspNetCoreModule);

        // Assert
        Assert.Equal("HMS.Essentials.AspNetCore", moduleType.Namespace);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldBePublic()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAspNetCoreModule);

        // Assert
        Assert.True(moduleType.IsPublic);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldNotBeAbstract()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAspNetCoreModule);

        // Assert
        Assert.False(moduleType.IsAbstract);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldNotBeSealed()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAspNetCoreModule);

        // Assert
        Assert.False(moduleType.IsSealed);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_ShouldHaveParameterlessConstructor()
    {
        // Arrange & Act
        var constructor = typeof(EssentialsAspNetCoreModule).GetConstructor(Type.EmptyTypes);

        // Assert
        Assert.NotNull(constructor);
        Assert.True(constructor.IsPublic);
    }

    [Fact]
    public void EssentialsAspNetCoreModule_CanBeUsedInModuleChain()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var appBuilder = new ApplicationBuilder(services, configuration);

        // Act
        var applicationHost = appBuilder
            .UseRootModule<TestModuleWithAspNetCoreDependency>()
            .Build();

        // Assert
        Assert.NotNull(applicationHost);
        Assert.Contains(applicationHost.Modules, m => m.ModuleType == typeof(EssentialsAspNetCoreModule));
        Assert.Contains(applicationHost.Modules, m => m.ModuleType == typeof(EssentialsCoreModule));
    }

    [Fact]
    public void EssentialsAspNetCoreModule_MultipleDependencies_ShouldLoadInCorrectOrder()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var appBuilder = new ApplicationBuilder(services, configuration);

        // Act
        var applicationHost = appBuilder
            .UseRootModule<TestModuleWithAspNetCoreDependency>()
            .Build();

        // Assert
        var moduleTypes = applicationHost.Modules.Select(m => m.ModuleType.Name).ToList();
        var coreIndex = moduleTypes.IndexOf(nameof(EssentialsCoreModule));
        var aspNetCoreIndex = moduleTypes.IndexOf(nameof(EssentialsAspNetCoreModule));
        var testIndex = moduleTypes.IndexOf(nameof(TestModuleWithAspNetCoreDependency));

        Assert.True(coreIndex >= 0, "EssentialsCoreModule not found");
        Assert.True(aspNetCoreIndex >= 0, "EssentialsAspNetCoreModule not found");
        Assert.True(testIndex >= 0, "TestModuleWithAspNetCoreDependency not found");
        Assert.True(coreIndex < aspNetCoreIndex, "Core should load before AspNetCore");
        Assert.True(aspNetCoreIndex < testIndex, "AspNetCore should load before Test");
    }

    // Test helper module
    [DependsOn(typeof(EssentialsAspNetCoreModule))]
    private class TestModuleWithAspNetCoreDependency : EssentialsModule
    {
    }
}
