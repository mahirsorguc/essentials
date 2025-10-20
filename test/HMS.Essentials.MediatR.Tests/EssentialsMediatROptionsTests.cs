using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

public class EssentialsMediatROptionsTests
{
    [Fact]
    public void Options_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new EssentialsMediatROptions();

        // Assert
        options.FluentValidationEnabled.ShouldBeFalse();
        options.UnitOfWorkEnabled.ShouldBeFalse();
        options.PerformanceLoggingEnabled.ShouldBeFalse();
        options.RequestLoggingEnabled.ShouldBeFalse();
    }

    [Fact]
    public void FluentValidationEnabled_ShouldBeSettable()
    {
        // Arrange
        var options = new EssentialsMediatROptions();

        // Act
        options.FluentValidationEnabled = true;

        // Assert
        options.FluentValidationEnabled.ShouldBeTrue();
    }

    [Fact]
    public void UnitOfWorkEnabled_ShouldBeSettable()
    {
        // Arrange
        var options = new EssentialsMediatROptions();

        // Act
        options.UnitOfWorkEnabled = true;

        // Assert
        options.UnitOfWorkEnabled.ShouldBeTrue();
    }

    [Fact]
    public void PerformanceLoggingEnabled_ShouldBeSettable()
    {
        // Arrange
        var options = new EssentialsMediatROptions();

        // Act
        options.PerformanceLoggingEnabled = true;

        // Assert
        options.PerformanceLoggingEnabled.ShouldBeTrue();
    }

    [Fact]
    public void RequestLoggingEnabled_ShouldBeSettable()
    {
        // Arrange
        var options = new EssentialsMediatROptions();

        // Act
        options.RequestLoggingEnabled = true;

        // Assert
        options.RequestLoggingEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Options_ShouldAllowMultiplePropertiesEnabled()
    {
        // Arrange
        var options = new EssentialsMediatROptions
        {
            FluentValidationEnabled = true,
            UnitOfWorkEnabled = true,
            PerformanceLoggingEnabled = true,
            RequestLoggingEnabled = true
        };

        // Act & Assert
        options.FluentValidationEnabled.ShouldBeTrue();
        options.UnitOfWorkEnabled.ShouldBeTrue();
        options.PerformanceLoggingEnabled.ShouldBeTrue();
        options.RequestLoggingEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Options_ShouldAllowSelectiveEnabling()
    {
        // Arrange
        var options = new EssentialsMediatROptions
        {
            FluentValidationEnabled = true,
            UnitOfWorkEnabled = false,
            PerformanceLoggingEnabled = true,
            RequestLoggingEnabled = false
        };

        // Act & Assert
        options.FluentValidationEnabled.ShouldBeTrue();
        options.UnitOfWorkEnabled.ShouldBeFalse();
        options.PerformanceLoggingEnabled.ShouldBeTrue();
        options.RequestLoggingEnabled.ShouldBeFalse();
    }
}
