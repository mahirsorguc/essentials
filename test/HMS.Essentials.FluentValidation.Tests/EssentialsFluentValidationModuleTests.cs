using FluentValidation;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.FluentValidation;

/// <summary>
/// Tests for <see cref="EssentialsFluentValidationModule"/>.
/// </summary>
public class EssentialsFluentValidationModuleTests
{
    public class TestDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestDtoValidator : AbstractValidator<TestDto>
    {
        public TestDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public void Module_Should_Implement_EssentialsModule()
    {
        // Arrange & Act
        var module = new EssentialsFluentValidationModule();

        // Assert
        module.ShouldBeAssignableTo<EssentialsModule>();
    }

    [Fact]
    public void ConfigureServices_Should_Register_Validators_When_Assembly_Is_Configured()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AssembliesToScan:0"] = "HMS.Essentials.FluentValidation.Tests"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var validator = serviceProvider.GetService<IValidator<TestDto>>();
        validator.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_Should_Not_Register_Validators_When_No_Assembly_Configured()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var validator = serviceProvider.GetService<IValidator<TestDto>>();
        validator.ShouldBeNull();
    }

    [Fact]
    public void ConfigureServices_Should_Use_Default_Configuration_When_Not_Provided()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }

    [Fact]
    public void ConfigureServices_Should_Register_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:MaxValidationDepth"] = "20",
            ["FluentValidation:StopOnFirstFailure"] = "true"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var config = serviceProvider.GetService<FluentValidationConfiguration>();
        config.ShouldNotBeNull();
        config.MaxValidationDepth.ShouldBe(20);
        config.StopOnFirstFailure.ShouldBeTrue();
    }

    [Fact]
    public void ConfigureServices_Should_Handle_Multiple_Assemblies()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AssembliesToScan:0"] = "HMS.Essentials.FluentValidation.Tests",
            ["FluentValidation:AssembliesToScan:1"] = "HMS.Essentials.FluentValidation"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var validator = serviceProvider.GetService<IValidator<TestDto>>();
        validator.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_Should_Handle_Invalid_Assembly_Names_Gracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AssembliesToScan:0"] = "NonExistentAssembly"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }

    [Fact]
    public void Configuration_Should_Validate_MaxValidationDepth()
    {
        // Arrange
        var config = new FluentValidationConfiguration
        {
            MaxValidationDepth = 0
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => config.Validate());
    }

    [Fact]
    public void Configuration_Should_Reject_Excessive_MaxValidationDepth()
    {
        // Arrange
        var config = new FluentValidationConfiguration
        {
            MaxValidationDepth = 150
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => config.Validate());
    }

    [Fact]
    public void Configuration_Should_Accept_Valid_MaxValidationDepth()
    {
        // Arrange
        var config = new FluentValidationConfiguration
        {
            MaxValidationDepth = 50
        };

        // Act & Assert
        Should.NotThrow(() => config.Validate());
    }

    [Fact]
    public void Module_Should_Configure_AspNetCore_Validation_When_Enabled()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddControllers();
        
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AspNetCore:EnableAutoValidation"] = "true",
            ["FluentValidation:AssembliesToScan:0"] = "HMS.Essentials.FluentValidation.Tests"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var module = new EssentialsFluentValidationModule();
        var context = new ModuleContext(services, configuration);

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }
}
