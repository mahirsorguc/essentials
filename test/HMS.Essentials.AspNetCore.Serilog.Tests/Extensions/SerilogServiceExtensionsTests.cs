using HMS.Essentials.AspNetCore.Serilog.Configuration;
using HMS.Essentials.AspNetCore.Serilog.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace HMS.Essentials.AspNetCore.Serilog.Tests.Extensions;

/// <summary>
/// Tests for SerilogServiceExtensions.
/// </summary>
public class SerilogServiceExtensionsTests
{
    [Fact]
    public void AddSerilogLogging_WithDefaults_ShouldRegisterOptions()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddSerilogLogging();
        var app = builder.Build();

        // Assert
        var options = app.Services.GetService<IOptions<SerilogOptions>>();
        options.ShouldNotBeNull();
        options.Value.ShouldNotBeNull();
    }

    [Fact]
    public void AddSerilogLogging_WithConfiguration_ShouldApplySettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["Serilog:Enabled"] = "false";
        builder.Configuration["Serilog:MinimumLevel"] = "Debug";

        // Act
        builder.AddSerilogLogging();
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.Enabled.ShouldBeFalse();
        options.MinimumLevel.ShouldBe("Debug");
    }

    [Fact]
    public void AddSerilogLogging_WithProgrammaticConfiguration_ShouldOverrideSettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddSerilogLogging(options =>
        {
            options.MinimumLevel = "Warning";
            options.WriteToConsole = false;
        });
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.MinimumLevel.ShouldBe("Warning");
        options.WriteToConsole.ShouldBeFalse();
    }

    [Fact]
    public void AddSerilogLogging_WhenDisabled_ShouldStillRegisterOptions()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddSerilogLogging(options => options.Enabled = false);
        var app = builder.Build();

        // Assert
        var options = app.Services.GetService<IOptions<SerilogOptions>>();
        options.ShouldNotBeNull();
        options.Value.Enabled.ShouldBeFalse();
    }

    [Fact]
    public void AddSerilogLogging_ReturnsBuilder_ForMethodChaining()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        var result = builder.AddSerilogLogging();

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddSerilogLogging_WithComplexConfiguration_ShouldApplyAllSettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddSerilogLogging(options =>
        {
            options.MinimumLevel = "Debug";
            options.WriteToConsole = true;
            options.WriteToFile = true;
            options.LogFilePath = "CustomLogs/app-.log";
            options.RollingInterval = "Hour";
            options.RetainedFileCountLimit = 7;
            options.EnrichWithMachineName = false;
            options.EnrichWithThreadId = false;
            options.EnrichWithEnvironmentName = false;
        });
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
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
    public void UseEssentialsSerilogRequestLogging_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging();
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseEssentialsSerilogRequestLogging());
    }

    [Fact]
    public void UseEssentialsSerilogRequestLogging_ReturnsApp_ForMethodChaining()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddSerilogLogging();
        var app = builder.Build();

        // Act
        var result = app.UseEssentialsSerilogRequestLogging();

        // Assert
        result.ShouldBe(app);
    }

    [Fact]
    public void AddSerilogLogging_WithCustomOutputTemplate_ShouldApply()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var customTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

        // Act
        builder.AddSerilogLogging(options =>
        {
            options.ConsoleOutputTemplate = customTemplate;
            options.FileOutputTemplate = customTemplate;
        });
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.ConsoleOutputTemplate.ShouldBe(customTemplate);
        options.FileOutputTemplate.ShouldBe(customTemplate);
    }

    [Fact]
    public void AddSerilogLogging_WithMinimumLevelOverrides_ShouldApply()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddSerilogLogging(options =>
        {
            options.MinimumLevelOverrides.Clear();
            options.MinimumLevelOverrides["Custom.Namespace"] = "Debug";
            options.MinimumLevelOverrides["Another.Namespace"] = "Error";
        });
        var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        options.MinimumLevelOverrides.Count.ShouldBe(2);
        options.MinimumLevelOverrides["Custom.Namespace"].ShouldBe("Debug");
        options.MinimumLevelOverrides["Another.Namespace"].ShouldBe("Error");
    }
}
