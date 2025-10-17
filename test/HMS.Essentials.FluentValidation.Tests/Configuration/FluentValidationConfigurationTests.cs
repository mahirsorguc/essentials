using Microsoft.Extensions.Configuration;

namespace HMS.Essentials.FluentValidation.Configuration;

/// <summary>
/// Tests for <see cref="FluentValidationConfiguration"/>.
/// </summary>
public class FluentValidationConfigurationTests
{
    [Fact]
    public void Configuration_Should_Have_Default_Values()
    {
        // Arrange & Act
        var config = new FluentValidationConfiguration();

        // Assert
        config.AutomaticValidationEnabled.ShouldBeTrue();
        config.DisableImplicitChildValidatorInjection.ShouldBeFalse();
        config.ThrowOnValidationFailures.ShouldBeTrue();
        config.StopOnFirstFailure.ShouldBeFalse();
        config.MaxValidationDepth.ShouldBe(10);
        config.ValidateAsyncRulesSynchronously.ShouldBeFalse();
        config.UseDetailedErrors.ShouldBeTrue();
        config.CustomErrorMessages.ShouldBeEmpty();
        config.AssembliesToScan.ShouldBeEmpty();
        config.IncludeReferencedAssemblies.ShouldBeFalse();
        config.AspNetCore.ShouldNotBeNull();
    }

    [Fact]
    public void Configuration_Should_Load_From_IConfiguration()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AutomaticValidationEnabled"] = "false",
            ["FluentValidation:ThrowOnValidationFailures"] = "false",
            ["FluentValidation:StopOnFirstFailure"] = "true",
            ["FluentValidation:MaxValidationDepth"] = "20",
            ["FluentValidation:UseDetailedErrors"] = "false"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var config = configuration
            .GetSection("FluentValidation")
            .Get<FluentValidationConfiguration>();

        // Assert
        config.ShouldNotBeNull();
        config.AutomaticValidationEnabled.ShouldBeFalse();
        config.ThrowOnValidationFailures.ShouldBeFalse();
        config.StopOnFirstFailure.ShouldBeTrue();
        config.MaxValidationDepth.ShouldBe(20);
        config.UseDetailedErrors.ShouldBeFalse();
    }

    [Fact]
    public void Configuration_Should_Load_AssembliesToScan_From_IConfiguration()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AssembliesToScan:0"] = "Assembly1",
            ["FluentValidation:AssembliesToScan:1"] = "Assembly2",
            ["FluentValidation:AssembliesToScan:2"] = "Assembly3"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var config = configuration
            .GetSection("FluentValidation")
            .Get<FluentValidationConfiguration>();

        // Assert
        config.ShouldNotBeNull();
        config.AssembliesToScan.ShouldNotBeEmpty();
        config.AssembliesToScan.Count.ShouldBe(3);
        config.AssembliesToScan.ShouldContain("Assembly1");
        config.AssembliesToScan.ShouldContain("Assembly2");
        config.AssembliesToScan.ShouldContain("Assembly3");
    }

    [Fact]
    public void Configuration_Should_Load_CustomErrorMessages_From_IConfiguration()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:CustomErrorMessages:Required"] = "This field is required",
            ["FluentValidation:CustomErrorMessages:Email"] = "Invalid email address"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var config = configuration
            .GetSection("FluentValidation")
            .Get<FluentValidationConfiguration>();

        // Assert
        config.ShouldNotBeNull();
        config.CustomErrorMessages.ShouldNotBeEmpty();
        config.CustomErrorMessages.Count.ShouldBe(2);
        config.CustomErrorMessages["Required"].ShouldBe("This field is required");
        config.CustomErrorMessages["Email"].ShouldBe("Invalid email address");
    }

    [Fact]
    public void AspNetCoreValidationOptions_Should_Have_Default_Values()
    {
        // Arrange & Act
        var options = new AspNetCoreValidationOptions();

        // Assert
        options.EnableAutoValidation.ShouldBeTrue();
        options.DisableDataAnnotationsValidation.ShouldBeTrue(); // Default is true in implementation
        options.UseStandardErrorResponse.ShouldBeTrue();
        options.IncludeErrorDetails.ShouldBeTrue();
        options.EnableClientSideValidation.ShouldBeFalse();
        options.RunValidationAsynchronously.ShouldBeFalse(); // Default is false in implementation
        options.ImplicitlyValidateChildProperties.ShouldBeTrue();
        options.ImplicitlyValidateRootCollectionElements.ShouldBeTrue();
    }

    [Fact]
    public void AspNetCoreValidationOptions_Should_Load_From_IConfiguration()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:AspNetCore:EnableAutoValidation"] = "false",
            ["FluentValidation:AspNetCore:DisableDataAnnotationsValidation"] = "true",
            ["FluentValidation:AspNetCore:UseStandardErrorResponse"] = "false",
            ["FluentValidation:AspNetCore:IncludeErrorDetails"] = "false",
            ["FluentValidation:AspNetCore:EnableClientSideValidation"] = "true",
            ["FluentValidation:AspNetCore:RunValidationAsynchronously"] = "false",
            ["FluentValidation:AspNetCore:ImplicitlyValidateChildProperties"] = "false",
            ["FluentValidation:AspNetCore:ImplicitlyValidateRootCollectionElements"] = "false"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var config = configuration
            .GetSection("FluentValidation")
            .Get<FluentValidationConfiguration>();

        // Assert
        config.ShouldNotBeNull();
        config.AspNetCore.ShouldNotBeNull();
        config.AspNetCore.EnableAutoValidation.ShouldBeFalse();
        config.AspNetCore.DisableDataAnnotationsValidation.ShouldBeTrue();
        config.AspNetCore.UseStandardErrorResponse.ShouldBeFalse();
        config.AspNetCore.IncludeErrorDetails.ShouldBeFalse();
        config.AspNetCore.EnableClientSideValidation.ShouldBeTrue();
        config.AspNetCore.RunValidationAsynchronously.ShouldBeFalse();
        config.AspNetCore.ImplicitlyValidateChildProperties.ShouldBeFalse();
        config.AspNetCore.ImplicitlyValidateRootCollectionElements.ShouldBeFalse();
    }

    [Fact]
    public void Validate_Should_Throw_When_MaxValidationDepth_Is_Less_Than_One()
    {
        // Arrange
        var config = new FluentValidationConfiguration
        {
            MaxValidationDepth = 0
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => config.Validate());
        exception.Message.ShouldContain("MaxValidationDepth");
        exception.Message.ShouldContain("greater than 0");
    }

    [Fact]
    public void Validate_Should_Throw_When_MaxValidationDepth_Exceeds_Maximum()
    {
        // Arrange
        var config = new FluentValidationConfiguration
        {
            MaxValidationDepth = 150
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => config.Validate());
        exception.Message.ShouldContain("MaxValidationDepth");
        exception.Message.ShouldContain("must not exceed 100");
    }

    [Fact]
    public void Validate_Should_Pass_When_MaxValidationDepth_Is_Valid()
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
    public void Configuration_Should_Support_Property_Assignment()
    {
        // Arrange
        var config = new FluentValidationConfiguration();

        // Act
        config.AutomaticValidationEnabled = false;
        config.DisableImplicitChildValidatorInjection = true;
        config.ThrowOnValidationFailures = false;
        config.StopOnFirstFailure = true;
        config.MaxValidationDepth = 25;
        config.ValidateAsyncRulesSynchronously = true;
        config.UseDetailedErrors = false;
        config.IncludeReferencedAssemblies = true;
        config.AssembliesToScan = new List<string> { "TestAssembly" };
        config.CustomErrorMessages = new Dictionary<string, string> { ["Test"] = "TestMessage" };

        // Assert
        config.AutomaticValidationEnabled.ShouldBeFalse();
        config.DisableImplicitChildValidatorInjection.ShouldBeTrue();
        config.ThrowOnValidationFailures.ShouldBeFalse();
        config.StopOnFirstFailure.ShouldBeTrue();
        config.MaxValidationDepth.ShouldBe(25);
        config.ValidateAsyncRulesSynchronously.ShouldBeTrue();
        config.UseDetailedErrors.ShouldBeFalse();
        config.IncludeReferencedAssemblies.ShouldBeTrue();
        config.AssembliesToScan.Count.ShouldBe(1);
        config.CustomErrorMessages.Count.ShouldBe(1);
    }

    [Fact]
    public void AspNetCoreValidationOptions_Should_Support_Property_Assignment()
    {
        // Arrange
        var options = new AspNetCoreValidationOptions();

        // Act
        options.EnableAutoValidation = false;
        options.DisableDataAnnotationsValidation = true;
        options.UseStandardErrorResponse = false;
        options.IncludeErrorDetails = false;
        options.EnableClientSideValidation = true;
        options.RunValidationAsynchronously = false;
        options.ImplicitlyValidateChildProperties = false;
        options.ImplicitlyValidateRootCollectionElements = false;

        // Assert
        options.EnableAutoValidation.ShouldBeFalse();
        options.DisableDataAnnotationsValidation.ShouldBeTrue();
        options.UseStandardErrorResponse.ShouldBeFalse();
        options.IncludeErrorDetails.ShouldBeFalse();
        options.EnableClientSideValidation.ShouldBeTrue();
        options.RunValidationAsynchronously.ShouldBeFalse();
        options.ImplicitlyValidateChildProperties.ShouldBeFalse();
        options.ImplicitlyValidateRootCollectionElements.ShouldBeFalse();
    }

    [Fact]
    public void Configuration_Should_Handle_Null_Collections()
    {
        // Arrange
        var config = new FluentValidationConfiguration
        {
            AssembliesToScan = null!,
            CustomErrorMessages = null!
        };

        // Act & Assert
        config.AssembliesToScan.ShouldBeNull();
        config.CustomErrorMessages.ShouldBeNull();
    }

    [Fact]
    public void Configuration_Should_Handle_Partial_Configuration()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["FluentValidation:StopOnFirstFailure"] = "true"
            // Other properties not specified
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var config = configuration
            .GetSection("FluentValidation")
            .Get<FluentValidationConfiguration>();

        // Assert
        config.ShouldNotBeNull();
        config.StopOnFirstFailure.ShouldBeTrue();
        // Other properties should have default values from class initialization
        config.AutomaticValidationEnabled.ShouldBeTrue(); // Default is true
        config.MaxValidationDepth.ShouldBe(10); // Default is 10
    }

    [Fact]
    public void Configuration_Should_Handle_Empty_Collections()
    {
        // Arrange
        var config = new FluentValidationConfiguration();

        // Act & Assert
        config.AssembliesToScan.ShouldBeEmpty();
        config.CustomErrorMessages.ShouldBeEmpty();
        config.AssembliesToScan.ShouldNotBeNull();
        config.CustomErrorMessages.ShouldNotBeNull();
    }
}
