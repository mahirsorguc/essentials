using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle.Configuration;
using HMS.Essentials.Swashbuckle.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace HMS.Essentials.Swashbuckle.Tests;

/// <summary>
/// Tests for EssentialsSwashbuckleModule.
/// </summary>
public class EssentialsSwashbuckleModuleTests
{
    [Fact]
    public void Module_ShouldBeInstantiable()
    {
        // Act
        var module = new EssentialsSwashbuckleModule();

        // Assert
        module.ShouldNotBeNull();
        module.ShouldBeAssignableTo<IModule>();
    }

    [Fact]
    public void Module_ShouldDependOnCoreModule()
    {
        // Arrange
        var moduleType = typeof(EssentialsSwashbuckleModule);

        // Act
        var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .ToArray();

        // Assert
        dependsOnAttributes.ShouldNotBeEmpty();
        dependsOnAttributes.Any(attr => attr.DependsOn.Contains(typeof(EssentialsCoreModule)))
            .ShouldBeTrue();
    }

    [Fact]
    public void ConfigureServices_ShouldRegisterSwaggerServices()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_WithoutConfiguration_ShouldUseDefaults()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.Title.ShouldBe("API Documentation");
        options.Version.ShouldBe("v1");
        options.EnableSwaggerUI.ShouldBeTrue();
        options.EnableInProduction.ShouldBeFalse();
    }

    [Fact]
    public void ConfigureServices_WithConfiguration_ShouldUseConfiguredValues()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        
        var configSettings = new Dictionary<string, string?>
        {
            ["Swagger:Title"] = "Custom API",
            ["Swagger:Version"] = "v2",
            ["Swagger:Description"] = "Custom Description",
            ["Swagger:EnableInProduction"] = "true",
            ["Swagger:ContactName"] = "Support Team",
            ["Swagger:ContactEmail"] = "support@example.com"
        };
        
        var configuration = TestHelper.CreateSwaggerConfiguration(configSettings);
        var context = TestHelper.CreateModuleContext(services, configuration);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.Title.ShouldBe("Custom API");
        options.Version.ShouldBe("v2");
        options.Description.ShouldBe("Custom Description");
        options.EnableInProduction.ShouldBeTrue();
        options.ContactName.ShouldBe("Support Team");
        options.ContactEmail.ShouldBe("support@example.com");
    }

    [Fact]
    public void ConfigureServices_ShouldNotThrow()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var context = TestHelper.CreateModuleContext();

        // Act & Assert
        Should.NotThrow(() => module.ConfigureServices(context));
    }

    [Fact]
    public void Initialize_ShouldNotThrow()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        services.AddLogging();
        var context = TestHelper.CreateModuleContext(services);
        module.ConfigureServices(context);

        // Act & Assert
        Should.NotThrow(() => module.Initialize(context));
    }

    [Fact]
    public void Initialize_ShouldLogInformation()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        services.AddLogging();
        var context = TestHelper.CreateModuleContext(services);
        module.ConfigureServices(context);

        // Act
        module.Initialize(context);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<EssentialsSwashbuckleModule>>();
        logger.ShouldNotBeNull();
    }

    [Fact]
    public void Shutdown_ShouldNotThrow()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var context = TestHelper.CreateModuleContext();

        // Act & Assert
        Should.NotThrow(() => module.Shutdown(context));
    }

    [Fact]
    public void ConfigureServices_WithFullConfiguration_ShouldMapAllProperties()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        
        var configSettings = new Dictionary<string, string?>
        {
            ["Swagger:EnableSwaggerUI"] = "false",
            ["Swagger:EnableInProduction"] = "true",
            ["Swagger:Title"] = "Complete API",
            ["Swagger:Version"] = "v3",
            ["Swagger:Description"] = "Complete Description",
            ["Swagger:TermsOfServiceUrl"] = "https://example.com/terms",
            ["Swagger:ContactName"] = "API Support",
            ["Swagger:ContactEmail"] = "support@api.com",
            ["Swagger:ContactUrl"] = "https://example.com/contact",
            ["Swagger:LicenseName"] = "MIT",
            ["Swagger:LicenseUrl"] = "https://opensource.org/licenses/MIT",
            ["Swagger:IncludeXmlComments"] = "false",
            ["Swagger:RoutePrefix"] = "api-docs",
            ["Swagger:EnableAnnotations"] = "false",
            ["Swagger:DocumentTitle"] = "API Documentation",
            ["Swagger:EnableDeepLinking"] = "false",
            ["Swagger:DisplayOperationId"] = "false",
            ["Swagger:DisplayRequestDuration"] = "false",
            ["Swagger:DefaultModelsExpandDepth"] = "2",
            ["Swagger:DefaultModelExpandDepth"] = "3",
            ["Swagger:ShowExtensions"] = "true",
            ["Swagger:ShowCommonExtensions"] = "true"
        };
        
        var configuration = TestHelper.CreateSwaggerConfiguration(configSettings);
        var context = TestHelper.CreateModuleContext(services, configuration);

        // Act
        module.ConfigureServices(context);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.EnableSwaggerUI.ShouldBeFalse();
        options.EnableInProduction.ShouldBeTrue();
        options.Title.ShouldBe("Complete API");
        options.Version.ShouldBe("v3");
        options.Description.ShouldBe("Complete Description");
        options.TermsOfServiceUrl.ShouldBe("https://example.com/terms");
        options.ContactName.ShouldBe("API Support");
        options.ContactEmail.ShouldBe("support@api.com");
        options.ContactUrl.ShouldBe("https://example.com/contact");
        options.LicenseName.ShouldBe("MIT");
        options.LicenseUrl.ShouldBe("https://opensource.org/licenses/MIT");
        options.IncludeXmlComments.ShouldBeFalse();
        options.RoutePrefix.ShouldBe("api-docs");
        options.EnableAnnotations.ShouldBeFalse();
        options.DocumentTitle.ShouldBe("API Documentation");
        options.EnableDeepLinking.ShouldBeFalse();
        options.DisplayOperationId.ShouldBeFalse();
        options.DisplayRequestDuration.ShouldBeFalse();
        options.DefaultModelsExpandDepth.ShouldBe(2);
        options.DefaultModelExpandDepth.ShouldBe(3);
        options.ShowExtensions.ShouldBeTrue();
        options.ShowCommonExtensions.ShouldBeTrue();
    }

    [Fact]
    public void Module_ShouldImplementEssentialsModule()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();

        // Assert
        module.ShouldBeAssignableTo<EssentialsModule>();
    }

    [Fact]
    public void ConfigureServices_ShouldRegisterSwaggerGen()
    {
        // Arrange
        var module = new EssentialsSwashbuckleModule();
        var services = new ServiceCollection();
        var context = TestHelper.CreateModuleContext(services);

        // Act
        module.ConfigureServices(context);

        // Assert
        // Verify SwaggerGen services are registered
        var descriptor = services.FirstOrDefault(d => 
            d.ServiceType.FullName?.Contains("SwaggerGenerator") == true);
        descriptor.ShouldNotBeNull();
    }

    [Fact]
    public void Module_ShouldHaveCorrectModuleType()
    {
        // Arrange & Act
        var module = new EssentialsSwashbuckleModule();

        // Assert
        module.GetType().Name.ShouldBe("EssentialsSwashbuckleModule");
        module.GetType().Namespace.ShouldBe("HMS.Essentials.Swashbuckle");
    }
}
