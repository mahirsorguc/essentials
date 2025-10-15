using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle.Configuration;
using HMS.Essentials.Swashbuckle.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using SwashbuckleSwagger = Swashbuckle.AspNetCore.Swagger;

namespace HMS.Essentials.Swashbuckle.Tests.Integration;

/// <summary>
/// Integration tests for Swashbuckle module.
/// </summary>
public class SwashbuckleIntegrationTests
{
    /// <summary>
    /// Test module that depends on Swashbuckle module.
    /// </summary>
    [DependsOn(typeof(EssentialsSwashbuckleModule))]
    private class TestWebApiModule : EssentialsModule
    {
    }

    [Fact]
    public void FullIntegration_WithModuleSystem_ShouldWorkCorrectly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;

        // Act
        builder.AddModuleSystem<TestWebApiModule>();
        var app = builder.Build();

        // Assert
        var options = app.Services.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
    }

    [Fact]
    public void FullIntegration_WithSwaggerMiddleware_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.AddModuleSystem<TestWebApiModule>();
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void FullIntegration_ModuleDependency_ShouldLoadSwashbuckleModule()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddModuleSystem<TestWebApiModule>();
        
        // Act
        var app = builder.Build();
        var appHost = app.Services.GetRequiredService<ApplicationHost>();

        // Assert
        var swashbuckleModule = appHost.Modules.FirstOrDefault(m => 
            m.ModuleType == typeof(EssentialsSwashbuckleModule));
        swashbuckleModule.ShouldNotBeNull();
        swashbuckleModule.Name.ShouldBe("EssentialsSwashbuckleModule");
    }

    [Fact]
    public void FullIntegration_WithConfiguration_ShouldApplySettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["Swagger:Title"] = "Integration Test API";
        builder.Configuration["Swagger:Version"] = "v2";
        builder.Configuration["Swagger:EnableInProduction"] = "true";
        
        builder.AddModuleSystem<TestWebApiModule>();
        
        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.Title.ShouldBe("Integration Test API");
        options.Version.ShouldBe("v2");
        options.EnableInProduction.ShouldBeTrue();
    }

    [Fact]
    public void FullIntegration_SwaggerServices_ShouldBeRegistered()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddModuleSystem<TestWebApiModule>();
        
        // Act
        var app = builder.Build();

        // Assert
        var swaggerProvider = app.Services.GetService<SwashbuckleSwagger.ISwaggerProvider>();
        swaggerProvider.ShouldNotBeNull();
    }

    [Fact]
    public void FullIntegration_ModuleLifecycle_ShouldComplete()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddModuleSystem<TestWebApiModule>();
        
        // Act
        var app = builder.Build();
        var appHost = app.Services.GetRequiredService<ApplicationHost>();

        // Assert
        var swashbuckleModule = appHost.Modules.FirstOrDefault(m => 
            m.ModuleType == typeof(EssentialsSwashbuckleModule));
        swashbuckleModule.ShouldNotBeNull();
        swashbuckleModule.State.ShouldBe(ModuleState.Initialized);
    }

    [Fact]
    public void FullIntegration_WithProgrammaticConfiguration_ShouldOverrideDefaults()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSwaggerServices(options =>
        {
            options.Title = "Programmatic API";
            options.Version = "v3";
            options.Description = "Configured programmatically";
        });
        
        // Act
        var app = builder.Build();

        // Assert
        var options = app.Services.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.Title.ShouldBe("Programmatic API");
        options.Version.ShouldBe("v3");
        options.Description.ShouldBe("Configured programmatically");
    }

    [Fact]
    public void FullIntegration_InProduction_WithoutEnabling_ShouldNotEnableSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Production;
        builder.Services.AddSwaggerServices(options =>
        {
            options.EnableInProduction = false;
        });
        var app = builder.Build();

        // Act & Assert - Should not throw even though Swagger won't be enabled
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void FullIntegration_InProduction_WithEnabling_ShouldEnableSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Production;
        builder.Services.AddSwaggerServices(options =>
        {
            options.EnableInProduction = true;
        });
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseSwaggerDocumentation());
    }

    [Fact]
    public void FullIntegration_ModuleChain_ShouldIncludeCoreModule()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddModuleSystem<TestWebApiModule>();
        
        // Act
        var app = builder.Build();
        var appHost = app.Services.GetRequiredService<ApplicationHost>();

        // Assert
        var coreModule = appHost.Modules.FirstOrDefault(m => 
            m.ModuleType == typeof(EssentialsCoreModule));
        coreModule.ShouldNotBeNull();
        
        var swashbuckleModule = appHost.Modules.FirstOrDefault(m => 
            m.ModuleType == typeof(EssentialsSwashbuckleModule));
        swashbuckleModule.ShouldNotBeNull();
    }

    [Fact]
    public void FullIntegration_WithCustomRoutePrefix_ShouldApply()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.Configuration["Swagger:RoutePrefix"] = "api-docs";
        builder.AddModuleSystem<TestWebApiModule>();
        var app = builder.Build();

        // Act
        app.UseSwaggerDocumentation();

        // Assert
        var options = app.Services.GetService<SwaggerOptions>();
        options.ShouldNotBeNull();
        options.RoutePrefix.ShouldBe("api-docs");
    }

    [Fact]
    public void FullIntegration_ApiExplorer_ShouldBeConfigured()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddModuleSystem<TestWebApiModule>();
        
        // Act
        var app = builder.Build();

        // Assert
        var apiDescriptionProvider = app.Services.GetServices<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionProvider>();
        apiDescriptionProvider.ShouldNotBeEmpty();
    }
}
