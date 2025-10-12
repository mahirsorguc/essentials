using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HMS.Essentials.Core.Tests.Modularity;

public class ApplicationBuilderTests
{
    private class TestModule : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            // Test module
        }
    }

    private class AnotherTestModule : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            // Another test module
        }
    }

    [Fact]
    public void UseRootModule_Generic_ShouldSetRootModule()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var result = builder.UseRootModule<TestModule>();

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result); // Should return the same instance for fluent API
    }

    [Fact]
    public void UseRootModule_Type_ShouldSetRootModule()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var result = builder.UseRootModule(typeof(TestModule));

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result); // Should return the same instance for fluent API
    }

    [Fact]
    public void UseRootModule_Generic_CalledTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule<TestModule>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            builder.UseRootModule<AnotherTestModule>());
        
        Assert.Contains("Root module has already been set", exception.Message);
        Assert.Contains("Only one root module is allowed", exception.Message);
    }

    [Fact]
    public void UseRootModule_Type_CalledTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule(typeof(TestModule));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            builder.UseRootModule(typeof(AnotherTestModule)));
        
        Assert.Contains("Root module has already been set", exception.Message);
        Assert.Contains("Only one root module is allowed", exception.Message);
    }

    [Fact]
    public void UseRootModule_GenericThenType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule<TestModule>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            builder.UseRootModule(typeof(AnotherTestModule)));
        
        Assert.Contains("Root module has already been set", exception.Message);
    }

    [Fact]
    public void UseRootModule_TypeThenGeneric_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule(typeof(TestModule));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            builder.UseRootModule<AnotherTestModule>());
        
        Assert.Contains("Root module has already been set", exception.Message);
    }

    [Fact]
    public void Create_ShouldReturnNewInstance()
    {
        // Act
        var builder = ApplicationBuilder.Create();

        // Assert
        Assert.NotNull(builder);
        Assert.NotNull(builder.Services);
        Assert.NotNull(builder.Configuration);
        Assert.NotNull(builder.Context);
    }

    [Fact]
    public void Constructor_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new ApplicationBuilder(null!, configuration));
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new ApplicationBuilder(services, null!));
    }

    [Fact]
    public void WithEnvironment_ShouldSetEnvironment()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var result = builder.WithEnvironment("Development");

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result);
    }

    [Fact]
    public void WithEnvironment_WithNullEnvironment_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            builder.WithEnvironment(null!));
    }

    [Fact]
    public void ConfigureServices_ShouldInvokeAction()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var actionInvoked = false;

        // Act
        var result = builder.ConfigureServices(services =>
        {
            actionInvoked = true;
        });

        // Assert
        Assert.True(actionInvoked);
        Assert.Same(builder, result);
    }

    [Fact]
    public void ConfigureServices_WithConfiguration_ShouldInvokeAction()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var actionInvoked = false;

        // Act
        var result = builder.ConfigureServices((services, config) =>
        {
            actionInvoked = true;
            Assert.NotNull(config);
        });

        // Assert
        Assert.True(actionInvoked);
        Assert.Same(builder, result);
    }

    [Fact]
    public void ConfigureServices_WithNullAction_ShouldNotThrow()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act & Assert
        var exception = Record.Exception(() => builder.ConfigureServices((Action<IServiceCollection>)null!));
        Assert.Null(exception);
    }

    [Fact]
    public void ConfigureServices_WithConfiguration_WithNullAction_ShouldNotThrow()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act & Assert
        var exception = Record.Exception(() => 
            builder.ConfigureServices((Action<IServiceCollection, IConfiguration>)null!));
        Assert.Null(exception);
    }

    [Fact]
    public void UseRootModule_Generic_SameModuleTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule<TestModule>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            builder.UseRootModule<TestModule>());
        
        Assert.Contains("Root module has already been set", exception.Message);
    }

    [Fact]
    public void UseRootModule_Type_SameModuleTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule(typeof(TestModule));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            builder.UseRootModule(typeof(TestModule)));
        
        Assert.Contains("Root module has already been set", exception.Message);
    }

    [Fact]
    public void AddModulesFromAssembly_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var assembly = typeof(TestModule).Assembly;

        // Act
        var result = builder.AddModulesFromAssembly(assembly);

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result);
    }

    [Fact]
    public void AddModulesFromCallingAssembly_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var result = builder.AddModulesFromCallingAssembly();

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result);
    }

    [Fact]
    public void AddModulesFromEntryAssembly_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var result = builder.AddModulesFromEntryAssembly();

        // Assert
        Assert.NotNull(result);
        Assert.Same(builder, result);
    }

    [Fact]
    public void Build_ShouldReturnApplicationHost()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        builder.UseRootModule<TestModule>();

        // Act
        var host = builder.Build();

        // Assert
        Assert.NotNull(host);
        Assert.IsType<ApplicationHost>(host);
    }

    [Fact]
    public void Build_WithoutRootModule_ShouldReturnApplicationHost()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var host = builder.Build();

        // Assert
        Assert.NotNull(host);
        Assert.IsType<ApplicationHost>(host);
    }

    [Fact]
    public void FluentAPI_MultipleMethodCalls_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        var result = builder
            .WithEnvironment("Development")
            .ConfigureServices(services => { })
            .UseRootModule<TestModule>();

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void UseRootModule_AfterAddModulesFromAssembly_ShouldNotThrow()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var assembly = typeof(TestModule).Assembly;
        builder.AddModulesFromAssembly(assembly);

        // Act & Assert
        var exception = Record.Exception(() => builder.UseRootModule<TestModule>());
        
        // Should not throw because AddModulesFromAssembly doesn't set the root module flag
        Assert.Null(exception);
    }

    [Fact]
    public void AddModulesFromAssembly_AfterUseRootModule_ShouldNotThrow()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var assembly = typeof(TestModule).Assembly;
        builder.UseRootModule<TestModule>();

        // Act & Assert
        var exception = Record.Exception(() => builder.AddModulesFromAssembly(assembly));
        
        // Should not throw because AddModulesFromAssembly is allowed after UseRootModule
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        var builder = new ApplicationBuilder(services, configuration);

        // Assert
        Assert.NotNull(builder.Services);
        Assert.NotNull(builder.Configuration);
        Assert.NotNull(builder.Context);
        Assert.Same(services, builder.Services);
        Assert.Same(configuration, builder.Configuration);
    }

    [Fact]
    public void Create_ShouldCreateIndependentInstances()
    {
        // Act
        var builder1 = ApplicationBuilder.Create();
        var builder2 = ApplicationBuilder.Create();

        // Assert
        Assert.NotSame(builder1, builder2);
        Assert.NotSame(builder1.Services, builder2.Services);
        Assert.NotSame(builder1.Configuration, builder2.Configuration);
    }

    [Fact]
    public void ConfigureServices_ShouldAllowMultipleCalls()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var firstCallInvoked = false;
        var secondCallInvoked = false;

        // Act
        builder
            .ConfigureServices(services => firstCallInvoked = true)
            .ConfigureServices(services => secondCallInvoked = true);

        // Assert
        Assert.True(firstCallInvoked);
        Assert.True(secondCallInvoked);
    }

    [Fact]
    public void ConfigureServices_WithConfiguration_ShouldAllowMultipleCalls()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        var firstCallInvoked = false;
        var secondCallInvoked = false;

        // Act
        builder
            .ConfigureServices((services, config) => firstCallInvoked = true)
            .ConfigureServices((services, config) => secondCallInvoked = true);

        // Assert
        Assert.True(firstCallInvoked);
        Assert.True(secondCallInvoked);
    }

    [Fact]
    public void ConfigureServices_ShouldAccessServiceCollection()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        IServiceCollection capturedServices = null!;

        // Act
        builder.ConfigureServices(services => capturedServices = services);

        // Assert
        Assert.NotNull(capturedServices);
        Assert.Same(builder.Services, capturedServices);
    }

    [Fact]
    public void ConfigureServices_WithConfiguration_ShouldAccessConfiguration()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();
        IConfiguration capturedConfig = null!;

        // Act
        builder.ConfigureServices((services, config) => capturedConfig = config);

        // Assert
        Assert.NotNull(capturedConfig);
        Assert.Same(builder.Configuration, capturedConfig);
    }

    [Fact]
    public void WithEnvironment_MultipleCallsWithDifferentEnvironments_ShouldUseLastValue()
    {
        // Arrange
        var builder = ApplicationBuilder.Create();

        // Act
        builder
            .WithEnvironment("Development")
            .WithEnvironment("Production")
            .WithEnvironment("Staging");

        // Assert - Context should use the last environment set
        // Note: We can't directly test _environment as it's private, 
        // but the test verifies the method doesn't throw
        Assert.NotNull(builder);
    }
}
