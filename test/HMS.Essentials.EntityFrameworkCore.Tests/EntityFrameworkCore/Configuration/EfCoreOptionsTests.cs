using FluentAssertions;

namespace HMS.Essentials.EntityFrameworkCore.Configuration;

public class EfCoreOptionsTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var options = new EfCoreOptions();

        // Assert
        options.DefaultConnectionString.Should().BeNull();
        options.ConnectionStrings.Should().NotBeNull();
        options.ConnectionStrings.Should().BeEmpty();
        options.EnableDetailedErrors.Should().BeFalse();
        options.EnableSensitiveDataLogging.Should().BeFalse();
        options.MaxRetryCount.Should().Be(6);
        options.MaxRetryDelay.Should().Be(30);
        options.UseLazyLoadingProxies.Should().BeFalse();
        options.CommandTimeout.Should().BeNull();
        options.AutoMigrateOnStartup.Should().BeFalse();
        options.SeedDataOnStartup.Should().BeFalse();
    }

    [Fact]
    public void DefaultConnectionString_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();
        var connectionString = "Server=localhost;Database=TestDb;";

        // Act
        options.DefaultConnectionString = connectionString;

        // Assert
        options.DefaultConnectionString.Should().Be(connectionString);
    }

    [Fact]
    public void ConnectionStrings_CanBeAdded()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.ConnectionStrings["Context1"] = "ConnectionString1";
        options.ConnectionStrings["Context2"] = "ConnectionString2";

        // Assert
        options.ConnectionStrings.Should().HaveCount(2);
        options.ConnectionStrings["Context1"].Should().Be("ConnectionString1");
        options.ConnectionStrings["Context2"].Should().Be("ConnectionString2");
    }

    [Fact]
    public void EnableDetailedErrors_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.EnableDetailedErrors = true;

        // Assert
        options.EnableDetailedErrors.Should().BeTrue();
    }

    [Fact]
    public void EnableSensitiveDataLogging_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.EnableSensitiveDataLogging = true;

        // Assert
        options.EnableSensitiveDataLogging.Should().BeTrue();
    }

    [Fact]
    public void MaxRetryCount_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.MaxRetryCount = 10;

        // Assert
        options.MaxRetryCount.Should().Be(10);
    }

    [Fact]
    public void MaxRetryDelay_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.MaxRetryDelay = 60;

        // Assert
        options.MaxRetryDelay.Should().Be(60);
    }

    [Fact]
    public void UseLazyLoadingProxies_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.UseLazyLoadingProxies = true;

        // Assert
        options.UseLazyLoadingProxies.Should().BeTrue();
    }

    [Fact]
    public void CommandTimeout_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.CommandTimeout = 120;

        // Assert
        options.CommandTimeout.Should().Be(120);
    }

    [Fact]
    public void AutoMigrateOnStartup_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.AutoMigrateOnStartup = true;

        // Assert
        options.AutoMigrateOnStartup.Should().BeTrue();
    }

    [Fact]
    public void SeedDataOnStartup_CanBeSet()
    {
        // Arrange
        var options = new EfCoreOptions();

        // Act
        options.SeedDataOnStartup = true;

        // Assert
        options.SeedDataOnStartup.Should().BeTrue();
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange
        var options = new EfCoreOptions
        {
            DefaultConnectionString = "Server=localhost;Database=TestDb;",
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true,
            MaxRetryCount = 10,
            MaxRetryDelay = 60,
            UseLazyLoadingProxies = true,
            CommandTimeout = 120,
            AutoMigrateOnStartup = true,
            SeedDataOnStartup = true
        };

        options.ConnectionStrings["Context1"] = "ConnectionString1";

        // Assert
        options.DefaultConnectionString.Should().Be("Server=localhost;Database=TestDb;");
        options.ConnectionStrings.Should().ContainKey("Context1");
        options.EnableDetailedErrors.Should().BeTrue();
        options.EnableSensitiveDataLogging.Should().BeTrue();
        options.MaxRetryCount.Should().Be(10);
        options.MaxRetryDelay.Should().Be(60);
        options.UseLazyLoadingProxies.Should().BeTrue();
        options.CommandTimeout.Should().Be(120);
        options.AutoMigrateOnStartup.Should().BeTrue();
        options.SeedDataOnStartup.Should().BeTrue();
    }
}
