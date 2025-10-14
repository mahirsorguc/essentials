using FluentAssertions;

namespace HMS.Essentials.EntityFrameworkCore.Configuration;

public class ConnectionStringProviderTests
{
    [Fact]
    public void Constructor_WithoutDefaultConnectionString_ShouldInitialize()
    {
        // Act
        var provider = new DefaultConnectionStringProvider();

        // Assert
        provider.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithDefaultConnectionString_ShouldInitialize()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";

        // Act
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);

        // Assert
        provider.Should().NotBeNull();
    }

    [Fact]
    public void GetConnectionString_WithoutContextName_ShouldReturnDefaultConnectionString()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);

        // Act
        var result = provider.GetConnectionString();

        // Assert
        result.Should().Be(defaultConnectionString);
    }

    [Fact]
    public void GetConnectionString_WithNullContextName_ShouldReturnDefaultConnectionString()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);

        // Act
        var result = provider.GetConnectionString(null);

        // Assert
        result.Should().Be(defaultConnectionString);
    }

    [Fact]
    public void GetConnectionString_WithEmptyContextName_ShouldReturnDefaultConnectionString()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);

        // Act
        var result = provider.GetConnectionString(string.Empty);

        // Assert
        result.Should().Be(defaultConnectionString);
    }

    [Fact]
    public void GetConnectionString_WithWhitespaceContextName_ShouldReturnDefaultConnectionString()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);

        // Act
        var result = provider.GetConnectionString("   ");

        // Assert
        result.Should().Be(defaultConnectionString);
    }

    [Fact]
    public void GetConnectionString_WithoutDefaultAndNoContextName_ShouldReturnNull()
    {
        // Arrange
        var provider = new DefaultConnectionStringProvider();

        // Act
        var result = provider.GetConnectionString();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddConnectionString_ShouldAddConnectionString()
    {
        // Arrange
        var provider = new DefaultConnectionStringProvider();
        var contextName = "TestContext";
        var connectionString = "Server=localhost;Database=TestDb;";

        // Act
        provider.AddConnectionString(contextName, connectionString);
        var result = provider.GetConnectionString(contextName);

        // Assert
        result.Should().Be(connectionString);
    }

    [Fact]
    public void AddConnectionString_MultipleTimes_ShouldUpdateConnectionString()
    {
        // Arrange
        var provider = new DefaultConnectionStringProvider();
        var contextName = "TestContext";
        var connectionString1 = "Server=localhost;Database=TestDb1;";
        var connectionString2 = "Server=localhost;Database=TestDb2;";

        // Act
        provider.AddConnectionString(contextName, connectionString1);
        provider.AddConnectionString(contextName, connectionString2);
        var result = provider.GetConnectionString(contextName);

        // Assert
        result.Should().Be(connectionString2);
    }

    [Fact]
    public void GetConnectionString_WithExistingContextName_ShouldReturnConnectionString()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);
        var contextName = "CustomContext";
        var customConnectionString = "Server=localhost;Database=CustomDb;";
        provider.AddConnectionString(contextName, customConnectionString);

        // Act
        var result = provider.GetConnectionString(contextName);

        // Assert
        result.Should().Be(customConnectionString);
    }

    [Fact]
    public void GetConnectionString_WithNonExistingContextName_ShouldReturnDefaultConnectionString()
    {
        // Arrange
        var defaultConnectionString = "Server=localhost;Database=DefaultDb;";
        var provider = new DefaultConnectionStringProvider(defaultConnectionString);
        provider.AddConnectionString("Context1", "ConnectionString1");

        // Act
        var result = provider.GetConnectionString("NonExistingContext");

        // Assert
        result.Should().Be(defaultConnectionString);
    }

    [Fact]
    public void GetConnectionString_CaseInsensitiveContextName_ShouldReturnConnectionString()
    {
        // Arrange
        var provider = new DefaultConnectionStringProvider();
        var connectionString = "Server=localhost;Database=TestDb;";
        provider.AddConnectionString("TestContext", connectionString);

        // Act
        var result1 = provider.GetConnectionString("TestContext");
        var result2 = provider.GetConnectionString("testcontext");
        var result3 = provider.GetConnectionString("TESTCONTEXT");

        // Assert
        result1.Should().Be(connectionString);
        result2.Should().Be(connectionString);
        result3.Should().Be(connectionString);
    }

    [Fact]
    public void AddConnectionString_MultipleContexts_ShouldHandleAllCorrectly()
    {
        // Arrange
        var provider = new DefaultConnectionStringProvider("DefaultConnection");
        var contexts = new Dictionary<string, string>
        {
            { "Context1", "ConnectionString1" },
            { "Context2", "ConnectionString2" },
            { "Context3", "ConnectionString3" }
        };

        // Act
        foreach (var kvp in contexts)
        {
            provider.AddConnectionString(kvp.Key, kvp.Value);
        }

        // Assert
        foreach (var kvp in contexts)
        {
            var result = provider.GetConnectionString(kvp.Key);
            result.Should().Be(kvp.Value);
        }
    }

    [Fact]
    public void IConnectionStringProvider_Interface_ShouldBeImplemented()
    {
        // Arrange & Act
        var provider = new DefaultConnectionStringProvider();

        // Assert
        provider.Should().BeAssignableTo<IConnectionStringProvider>();
    }
}
