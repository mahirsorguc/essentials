using HMS.Essentials.AspNetCore.Serilog.Configuration;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;

namespace HMS.Essentials.AspNetCore.Serilog.Tests;

/// <summary>
/// Tests for EssentialsAspNetCoreSerilogModule.
/// </summary>
public class EssentialsAspNetCoreSerilogModuleTests
{
    private static IConfiguration CreateConfiguration(Dictionary<string, string?>? settings = null)
    {
        var configBuilder = new ConfigurationBuilder();
        if (settings != null)
        {
            configBuilder.AddInMemoryCollection(settings);
        }
        return configBuilder.Build();
    }

    [Fact]
    public void Module_ShouldHaveCorrectDependencies()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAspNetCoreSerilogModule);
        var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .FirstOrDefault();

        // Assert
        dependsOnAttribute.ShouldNotBeNull();
        dependsOnAttribute.DependsOn.ShouldContain(typeof(EssentialsAspNetCoreModule));
    }

    [Fact]
    public void ConfigureServices_ShouldRegisterSerilogOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["Serilog:Enabled"] = "true",
            ["Serilog:MinimumLevel"] = "Information"
        });
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreSerilogModule();

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<IOptions<SerilogOptions>>();
        options.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_WithConfiguration_ShouldBindOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["Serilog:Enabled"] = "false",
            ["Serilog:MinimumLevel"] = "Debug",
            ["Serilog:WriteToConsole"] = "false"
        });
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreSerilogModule();

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.Enabled.ShouldBeFalse();
        options.MinimumLevel.ShouldBe("Debug");
        options.WriteToConsole.ShouldBeFalse();
    }

    [Fact]
    public void Initialize_WithLoggerFactory_ShouldLogInitialization()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = CreateConfiguration();
        var serviceProvider = services.BuildServiceProvider();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreSerilogModule();

        // Act & Assert
        Should.NotThrow(() => module.Initialize(context));
    }

    [Fact]
    public void Initialize_WithoutLoggerFactory_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreSerilogModule();

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
        var module = new EssentialsAspNetCoreSerilogModule();

        // Act & Assert
        Should.NotThrow(() => module.Shutdown(context));
    }

    [Fact]
    public void ConfigureServices_WithEmptyConfiguration_ShouldUseDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreSerilogModule();

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.Enabled.ShouldBeTrue();
        options.MinimumLevel.ShouldBe("Information");
    }

    [Fact]
    public void ConfigureServices_WithComplexConfiguration_ShouldBindAllProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["Serilog:Enabled"] = "true",
            ["Serilog:MinimumLevel"] = "Debug",
            ["Serilog:WriteToConsole"] = "true",
            ["Serilog:WriteToFile"] = "true",
            ["Serilog:LogFilePath"] = "CustomLogs/app-.log",
            ["Serilog:RollingInterval"] = "Hour",
            ["Serilog:RetainedFileCountLimit"] = "7",
            ["Serilog:EnrichWithMachineName"] = "false",
            ["Serilog:EnrichWithThreadId"] = "false",
            ["Serilog:EnrichWithEnvironmentName"] = "false"
        });
        var context = new ModuleContext(services, configuration);
        var module = new EssentialsAspNetCoreSerilogModule();

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.Enabled.ShouldBeTrue();
        options.MinimumLevel.ShouldBe("Debug");
        options.WriteToConsole.ShouldBeTrue();
        options.WriteToFile.ShouldBeTrue();
        options.LogFilePath.ShouldBe("CustomLogs/app-.log");
        options.RollingInterval.ShouldBe("Hour");
        options.RetainedFileCountLimit.ShouldBe(7);
        options.EnrichWithMachineName.ShouldBeFalse();
        options.EnrichWithThreadId.ShouldBeFalse();
        options.EnrichWithEnvironmentName.ShouldBeFalse();
    }

    [Fact]
    public void Module_CanBeInstantiated()
    {
        // Act & Assert
        Should.NotThrow(() => new EssentialsAspNetCoreSerilogModule());
    }

    [Fact]
    public void Module_InheritsFromEssentialsModule()
    {
        // Arrange
        var module = new EssentialsAspNetCoreSerilogModule();

        // Assert
        module.ShouldBeAssignableTo<EssentialsModule>();
    }
}
