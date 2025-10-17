using HMS.Essentials.AspNetCore.Serilog.Configuration;
using HMS.Essentials.AspNetCore.Serilog.Extensions;
using HMS.Essentials.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace HMS.Essentials.AspNetCore.Serilog.Tests.Integration;

/// <summary>
/// Integration tests for Serilog module.
/// </summary>
public class SerilogIntegrationTests
{
    [DependsOn(typeof(EssentialsAspNetCoreSerilogModule))]
    private class TestWebApiModule : EssentialsModule
    {
    }

    [Fact]
    public void FullIntegration_WithModuleSystem_ShouldWorkCorrectly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging();
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetService<IOptions<SerilogOptions>>();
        options.ShouldNotBeNull();
        options.Value.ShouldNotBeNull();
    }

    [Fact]
    public void FullIntegration_ModuleDependency_ShouldLoadSerilogModule()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging();
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();
        var appHost = app.GetApplicationHost();

        // Assert
        appHost.Modules.ShouldNotBeEmpty();
        appHost.Modules.ShouldContain(m => m.ModuleType == typeof(EssentialsAspNetCoreSerilogModule));
    }

    [Fact]
    public void FullIntegration_WithConfiguration_ShouldApplySettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["Serilog:MinimumLevel"] = "Debug";
        builder.Configuration["Serilog:WriteToConsole"] = "true";

        builder.AddSerilogLogging();
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.MinimumLevel.ShouldBe("Debug");
        options.WriteToConsole.ShouldBeTrue();
    }

    [Fact]
    public void FullIntegration_WithRequestLogging_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging();
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();

        // Assert
        Should.NotThrow(() => app.UseEssentialsSerilogRequestLogging());
    }

    [Fact]
    public void FullIntegration_WithProgrammaticConfiguration_ShouldOverrideDefaults()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging(options =>
        {
            options.MinimumLevel = "Warning";
            options.WriteToFile = false;
        });
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.MinimumLevel.ShouldBe("Warning");
        options.WriteToFile.ShouldBeFalse();
    }

    [Fact]
    public void FullIntegration_WhenDisabled_ShouldStillWorkCorrectly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging(options => options.Enabled = false);
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.Enabled.ShouldBeFalse();
    }

    [Fact]
    public void FullIntegration_MultipleModules_ShouldInitializeCorrectly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging();
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();
        var appHost = app.GetApplicationHost();

        // Assert
        appHost.Modules.ShouldNotBeEmpty();
        foreach (var module in appHost.Modules)
        {
            module.State.ShouldBe(ModuleState.Initialized);
        }
    }

    [Fact]
    public void FullIntegration_WithComplexConfiguration_ShouldApplyAllSettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging(options =>
        {
            options.MinimumLevel = "Debug";
            options.WriteToConsole = true;
            options.WriteToFile = true;
            options.LogFilePath = "IntegrationLogs/test-.log";
            options.RollingInterval = "Hour";
            options.RetainedFileCountLimit = 5;
            options.EnrichWithMachineName = true;
            options.EnrichWithThreadId = true;
            options.EnrichWithEnvironmentName = true;
        });
        builder.AddModuleSystem<TestWebApiModule>();

        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.MinimumLevel.ShouldBe("Debug");
        options.WriteToConsole.ShouldBeTrue();
        options.WriteToFile.ShouldBeTrue();
        options.LogFilePath.ShouldBe("IntegrationLogs/test-.log");
        options.RollingInterval.ShouldBe("Hour");
        options.RetainedFileCountLimit.ShouldBe(5);
        options.EnrichWithMachineName.ShouldBeTrue();
        options.EnrichWithThreadId.ShouldBeTrue();
        options.EnrichWithEnvironmentName.ShouldBeTrue();
    }
}
