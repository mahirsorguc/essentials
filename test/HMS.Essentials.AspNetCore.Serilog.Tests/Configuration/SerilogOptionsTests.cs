using HMS.Essentials.AspNetCore.Serilog.Configuration;
using Shouldly;

namespace HMS.Essentials.AspNetCore.Serilog.Tests.Configuration;

/// <summary>
/// Tests for SerilogOptions configuration class.
/// </summary>
public class SerilogOptionsTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var options = new SerilogOptions();

        // Assert
        options.Enabled.ShouldBeTrue();
        options.MinimumLevel.ShouldBe("Information");
        options.WriteToConsole.ShouldBeTrue();
        options.WriteToFile.ShouldBeTrue();
        options.LogFilePath.ShouldBe("Logs/log-.txt");
        options.RollingInterval.ShouldBe("Day");
        options.RetainedFileCountLimit.ShouldBe(31);
        options.FileSizeLimitBytes.ShouldBe(10485760);
        options.EnrichWithMachineName.ShouldBeTrue();
        options.EnrichWithThreadId.ShouldBeTrue();
        options.EnrichWithEnvironmentName.ShouldBeTrue();
        options.OverrideMinimumLevel.ShouldBeTrue();
        options.MinimumLevelOverrides.ShouldNotBeNull();
        options.MinimumLevelOverrides.Count.ShouldBe(3);
    }

    [Fact]
    public void Enabled_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.Enabled = false;

        // Assert
        options.Enabled.ShouldBeFalse();
    }

    [Fact]
    public void MinimumLevel_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.MinimumLevel = "Debug";

        // Assert
        options.MinimumLevel.ShouldBe("Debug");
    }

    [Fact]
    public void WriteToConsole_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.WriteToConsole = false;

        // Assert
        options.WriteToConsole.ShouldBeFalse();
    }

    [Fact]
    public void WriteToFile_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.WriteToFile = false;

        // Assert
        options.WriteToFile.ShouldBeFalse();
    }

    [Fact]
    public void LogFilePath_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.LogFilePath = "CustomLogs/app-.log";

        // Assert
        options.LogFilePath.ShouldBe("CustomLogs/app-.log");
    }

    [Fact]
    public void RollingInterval_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.RollingInterval = "Hour";

        // Assert
        options.RollingInterval.ShouldBe("Hour");
    }

    [Fact]
    public void RetainedFileCountLimit_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.RetainedFileCountLimit = 7;

        // Assert
        options.RetainedFileCountLimit.ShouldBe(7);
    }

    [Fact]
    public void FileSizeLimitBytes_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.FileSizeLimitBytes = 5242880; // 5MB

        // Assert
        options.FileSizeLimitBytes.ShouldBe(5242880);
    }

    [Fact]
    public void FileSizeLimitBytes_CanBeSetToNull()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.FileSizeLimitBytes = null;

        // Assert
        options.FileSizeLimitBytes.ShouldBeNull();
    }

    [Fact]
    public void EnrichWithMachineName_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.EnrichWithMachineName = false;

        // Assert
        options.EnrichWithMachineName.ShouldBeFalse();
    }

    [Fact]
    public void EnrichWithThreadId_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.EnrichWithThreadId = false;

        // Assert
        options.EnrichWithThreadId.ShouldBeFalse();
    }

    [Fact]
    public void EnrichWithEnvironmentName_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.EnrichWithEnvironmentName = false;

        // Assert
        options.EnrichWithEnvironmentName.ShouldBeFalse();
    }

    [Fact]
    public void ConsoleOutputTemplate_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();
        var template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

        // Act
        options.ConsoleOutputTemplate = template;

        // Assert
        options.ConsoleOutputTemplate.ShouldBe(template);
    }

    [Fact]
    public void FileOutputTemplate_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();
        var template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

        // Act
        options.FileOutputTemplate = template;

        // Assert
        options.FileOutputTemplate.ShouldBe(template);
    }

    [Fact]
    public void OverrideMinimumLevel_CanBeSet()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.OverrideMinimumLevel = false;

        // Assert
        options.OverrideMinimumLevel.ShouldBeFalse();
    }

    [Fact]
    public void MinimumLevelOverrides_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new SerilogOptions();

        // Assert
        options.MinimumLevelOverrides.ShouldContainKey("Microsoft.AspNetCore");
        options.MinimumLevelOverrides["Microsoft.AspNetCore"].ShouldBe("Warning");
        options.MinimumLevelOverrides.ShouldContainKey("Microsoft.EntityFrameworkCore");
        options.MinimumLevelOverrides["Microsoft.EntityFrameworkCore"].ShouldBe("Warning");
        options.MinimumLevelOverrides.ShouldContainKey("System");
        options.MinimumLevelOverrides["System"].ShouldBe("Warning");
    }

    [Fact]
    public void MinimumLevelOverrides_CanBeModified()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.MinimumLevelOverrides["Custom.Namespace"] = "Debug";
        options.MinimumLevelOverrides["Microsoft.AspNetCore"] = "Error";

        // Assert
        options.MinimumLevelOverrides["Custom.Namespace"].ShouldBe("Debug");
        options.MinimumLevelOverrides["Microsoft.AspNetCore"].ShouldBe("Error");
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange
        var options = new SerilogOptions();

        // Act
        options.Enabled = false;
        options.MinimumLevel = "Debug";
        options.WriteToConsole = false;
        options.WriteToFile = false;
        options.LogFilePath = "CustomLogs/app-.log";
        options.RollingInterval = "Hour";
        options.RetainedFileCountLimit = 7;
        options.FileSizeLimitBytes = null;
        options.EnrichWithMachineName = false;
        options.EnrichWithThreadId = false;
        options.EnrichWithEnvironmentName = false;
        options.ConsoleOutputTemplate = "Custom Console Template";
        options.FileOutputTemplate = "Custom File Template";
        options.OverrideMinimumLevel = false;
        options.MinimumLevelOverrides.Clear();

        // Assert
        options.Enabled.ShouldBeFalse();
        options.MinimumLevel.ShouldBe("Debug");
        options.WriteToConsole.ShouldBeFalse();
        options.WriteToFile.ShouldBeFalse();
        options.LogFilePath.ShouldBe("CustomLogs/app-.log");
        options.RollingInterval.ShouldBe("Hour");
        options.RetainedFileCountLimit.ShouldBe(7);
        options.FileSizeLimitBytes.ShouldBeNull();
        options.EnrichWithMachineName.ShouldBeFalse();
        options.EnrichWithThreadId.ShouldBeFalse();
        options.EnrichWithEnvironmentName.ShouldBeFalse();
        options.ConsoleOutputTemplate.ShouldBe("Custom Console Template");
        options.FileOutputTemplate.ShouldBe("Custom File Template");
        options.OverrideMinimumLevel.ShouldBeFalse();
        options.MinimumLevelOverrides.ShouldBeEmpty();
    }
}
