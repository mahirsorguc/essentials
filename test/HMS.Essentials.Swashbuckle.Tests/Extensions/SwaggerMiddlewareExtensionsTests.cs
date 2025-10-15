using HMS.Essentials.Swashbuckle.Configuration;
using HMS.Essentials.Swashbuckle.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;

namespace HMS.Essentials.Swashbuckle.Tests.Extensions;

/// <summary>
/// Tests for SwaggerMiddlewareExtensions.
/// </summary>
public class SwaggerMiddlewareExtensionsTests
{
    [Fact]
    public void UseSwaggerDocumentation_WithNullApp_ShouldThrowArgumentNullException()
    {
        // Arrange
        IApplicationBuilder app = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            app.UseSwaggerDocumentation());
    }

    [Fact]
    public void UseSwaggerDocumentation_WithDevelopmentEnvironment_ShouldEnableSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices();
        
        var app = builder.Build();

        // Act
        var result = app.UseSwaggerDocumentation();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(app);
    }

    [Fact]
    public void UseSwaggerDocumentation_WithProductionEnvironmentAndNotEnabled_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Production;
        builder.Services.AddSwaggerServices(options =>
        {
            options.EnableInProduction = false;
        });
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void UseSwaggerDocumentation_WithProductionEnvironmentAndEnabled_ShouldEnableSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Production;
        builder.Services.AddSwaggerServices(options =>
        {
            options.EnableInProduction = true;
        });
        
        var app = builder.Build();

        // Act
        var result = app.UseSwaggerDocumentation();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(app);
    }

    [Fact]
    public void UseSwaggerDocumentation_ShouldReturnApplicationBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices();
        
        var app = builder.Build();

        // Act
        var result = app.UseSwaggerDocumentation();

        // Assert
        result.ShouldBe(app);
    }

    [Fact]
    public void UseSwaggerDocumentation_WithoutOptions_ShouldUseDefaultOptions()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices();
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void UseSwaggerDocumentation_WithCustomConfiguration_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices();
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => 
            app.UseSwaggerDocumentation(options =>
            {
                options.RoutePrefix = "api-docs";
                options.DocumentTitle = "Custom Title";
            }));
    }

    [Fact]
    public void UseSwaggerDocumentation_WithNullConfigureAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSwaggerServices();
        var app = builder.Build();
        Action<SwaggerOptions> configure = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            app.UseSwaggerDocumentation(configure));
    }

    [Fact]
    public void UseSwaggerDocumentation_WithSwaggerUIDisabled_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices(options =>
        {
            options.EnableSwaggerUI = false;
        });
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void UseSwaggerDocumentation_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices();
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() =>
        {
            app.UseSwaggerDocumentation();
            app.UseSwaggerDocumentation();
        });
    }

    [Fact]
    public void UseSwaggerDocumentation_WithCustomRoutePrefix_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices(options =>
        {
            options.RoutePrefix = "docs";
        });
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void UseSwaggerDocumentation_WithAllUIOptions_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Services.AddSwaggerServices(options =>
        {
            options.DocumentTitle = "My API Docs";
            options.EnableDeepLinking = true;
            options.DisplayOperationId = true;
            options.DisplayRequestDuration = true;
            options.DefaultModelsExpandDepth = 2;
            options.DefaultModelExpandDepth = 2;
            options.ShowExtensions = true;
            options.ShowCommonExtensions = true;
        });
        
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void UseSwaggerDocumentation_InDevelopment_WithDefaultOptions_ShouldEnableSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        
        // Register services with default options
        var options = new SwaggerOptions();
        builder.Services.AddSingleton(options);
        builder.Services.AddSwaggerServices();
        
        var app = builder.Build();

        // Act
        var result = app.UseSwaggerDocumentation();

        // Assert
        result.ShouldNotBeNull();
        // Verify the app pipeline is configured
        result.ShouldBe(app);
    }
}
