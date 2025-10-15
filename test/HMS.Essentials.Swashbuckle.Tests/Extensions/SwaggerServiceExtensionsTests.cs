using HMS.Essentials.Swashbuckle.Configuration;
using HMS.Essentials.Swashbuckle.Extensions;
using HMS.Essentials.Swashbuckle.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SwashbuckleSwagger = Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HMS.Essentials.Swashbuckle.Tests.Extensions;

/// <summary>
/// Tests for SwaggerServiceExtensions.
/// </summary>
public class SwaggerServiceExtensionsTests
{
    [Fact]
    public void AddSwaggerServices_WithoutConfiguration_ShouldRegisterDefaultServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.Title.ShouldBe("API Documentation");
        options.Version.ShouldBe("v1");
    }

    [Fact]
    public void AddSwaggerServices_WithConfiguration_ShouldRegisterCustomOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.Title = "Custom API";
            options.Version = "v2";
            options.Description = "Custom Description";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredOptions = serviceProvider.GetService<SwaggerOptions>();
        registeredOptions.ShouldNotBeNull();
        registeredOptions.Title.ShouldBe("Custom API");
        registeredOptions.Version.ShouldBe("v2");
        registeredOptions.Description.ShouldBe("Custom Description");
    }

    [Fact]
    public void AddSwaggerServices_ShouldRegisterSwaggerGen()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices();

        // Assert
        // SwaggerGen should register services - check service descriptors
        var swaggerGenDescriptor = services.FirstOrDefault(d => 
            d.ServiceType.FullName?.Contains("ISwaggerProvider") == true);
        swaggerGenDescriptor.ShouldNotBeNull();
    }

    [Fact]
    public void AddSwaggerServices_ShouldRegisterApiExplorer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices();

        // Assert
        // Check if API Explorer services are registered
        var descriptor = services.FirstOrDefault(d => 
            d.ServiceType.FullName?.Contains("ApiDescriptionGroupCollectionProvider") == true);
        descriptor.ShouldNotBeNull();
    }

    [Fact]
    public void AddSwaggerServices_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            services.AddSwaggerServices());
    }

    [Fact]
    public void AddSwaggerServices_WithNullConfigureAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<SwaggerOptions> configure = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            services.AddSwaggerServices(configure));
    }

    [Fact]
    public void AddSwaggerServices_ShouldRegisterOptionsAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.Title = "Test API";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options1 = serviceProvider.GetService<SwaggerOptions>();
        var options2 = serviceProvider.GetService<SwaggerOptions>();
        options1.ShouldBe(options2); // Same instance
    }

    [Fact]
    public void AddSwaggerServices_WithContactInformation_ShouldConfigureCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.ContactName = "API Support";
            options.ContactEmail = "support@example.com";
            options.ContactUrl = "https://example.com/contact";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredOptions = serviceProvider.GetService<SwaggerOptions>();
        registeredOptions.ShouldNotBeNull();
        registeredOptions.ContactName.ShouldBe("API Support");
        registeredOptions.ContactEmail.ShouldBe("support@example.com");
        registeredOptions.ContactUrl.ShouldBe("https://example.com/contact");
    }

    [Fact]
    public void AddSwaggerServices_WithLicenseInformation_ShouldConfigureCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.LicenseName = "MIT";
            options.LicenseUrl = "https://opensource.org/licenses/MIT";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredOptions = serviceProvider.GetService<SwaggerOptions>();
        registeredOptions.ShouldNotBeNull();
        registeredOptions.LicenseName.ShouldBe("MIT");
        registeredOptions.LicenseUrl.ShouldBe("https://opensource.org/licenses/MIT");
    }

    [Fact]
    public void AddSwaggerServices_WithXmlCommentsDisabled_ShouldConfigureCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.IncludeXmlComments = false;
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredOptions = serviceProvider.GetService<SwaggerOptions>();
        registeredOptions.ShouldNotBeNull();
        registeredOptions.IncludeXmlComments.ShouldBeFalse();
    }

    [Fact]
    public void AddSwaggerServices_WithCustomRoutePrefix_ShouldConfigureCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.RoutePrefix = "api-docs";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredOptions = serviceProvider.GetService<SwaggerOptions>();
        registeredOptions.ShouldNotBeNull();
        registeredOptions.RoutePrefix.ShouldBe("api-docs");
    }

    [Fact]
    public void AddSwaggerServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddSwaggerServices();

        // Assert
        result.ShouldBe(services);
    }

    [Fact]
    public void AddSwaggerServices_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Should.NotThrow(() =>
        {
            services.AddSwaggerServices();
            services.AddSwaggerServices(options =>
            {
                options.Title = "Updated API";
            });
        });
    }

    [Fact]
    public void AddSwaggerServices_WithAllOptions_ShouldConfigureCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices(options =>
        {
            options.EnableSwaggerUI = false;
            options.EnableInProduction = true;
            options.Title = "Complete API";
            options.Version = "v3";
            options.Description = "Complete API Description";
            options.TermsOfServiceUrl = "https://example.com/terms";
            options.ContactName = "Support";
            options.ContactEmail = "support@example.com";
            options.ContactUrl = "https://example.com";
            options.LicenseName = "Apache 2.0";
            options.LicenseUrl = "https://www.apache.org/licenses/LICENSE-2.0";
            options.IncludeXmlComments = true;
            options.RoutePrefix = "docs";
            options.EnableAnnotations = true;
            options.CustomCss = "/custom.css";
            options.DocumentTitle = "API Documentation";
            options.EnableDeepLinking = true;
            options.DisplayOperationId = true;
            options.DisplayRequestDuration = true;
            options.DefaultModelsExpandDepth = 2;
            options.DefaultModelExpandDepth = 2;
            options.ShowExtensions = true;
            options.ShowCommonExtensions = true;
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredOptions = serviceProvider.GetService<SwaggerOptions>();
        registeredOptions.ShouldNotBeNull();
        registeredOptions.EnableSwaggerUI.ShouldBeFalse();
        registeredOptions.EnableInProduction.ShouldBeTrue();
        registeredOptions.Title.ShouldBe("Complete API");
        registeredOptions.Version.ShouldBe("v3");
        registeredOptions.Description.ShouldBe("Complete API Description");
    }
}
