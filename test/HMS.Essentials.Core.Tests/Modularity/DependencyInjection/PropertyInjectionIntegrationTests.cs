using HMS.Essentials.Modularity.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.Modularity.Tests.DependencyInjection;

/// <summary>
/// Integration tests for property injection within the module system.
/// </summary>
public class PropertyInjectionIntegrationTests
{
    [Fact]
    public void PropertyInjector_ShouldBeRegisteredInCoreModule()
    {
        // Arrange & Act
        var host = ApplicationBuilder.Create()
            .UseRootModule<TestModule>()
            .Build();

        // Assert
        var injector = host.Services.GetService<IPropertyInjector>();
        injector.ShouldNotBeNull();
        injector.ShouldBeOfType<PropertyInjector>();
    }

    [Fact]
    public void ModuleContext_InjectProperties_ShouldWorkInModuleInitialization()
    {
        // Arrange & Act
        var host = ApplicationBuilder.Create()
            .UseRootModule<TestModuleWithPropertyInjection>()
            .Build();

        // Assert
        var service = host.Services.GetRequiredService<TestServiceWithInjectedProperty>();
        service.Logger.ShouldNotBeNull();
    }

    [Fact]
    public void PropertyInjection_ShouldWorkWithModuleServices()
    {
        // Arrange & Act
        var host = ApplicationBuilder.Create()
            .UseRootModule<TestModuleWithPropertyInjection>()
            .Build();

        var service = host.Services.GetRequiredService<IComplexService>();

        // Assert
        service.ShouldBeOfType<ComplexService>();
        var complexService = (ComplexService)service;
        complexService.Logger.ShouldNotBeNull();
        complexService.Repository.ShouldNotBeNull();
    }

    [Fact]
    public void PropertyInjection_ShouldWorkWithMultipleModules()
    {
        // Arrange & Act
        var host = ApplicationBuilder.Create()
            .UseRootModule<DependentModule>()
            .Build();

        var service = host.Services.GetRequiredService<IDependentService>();

        // Assert
        service.ShouldBeOfType<DependentServiceImpl>();
        var dependentService = (DependentServiceImpl)service;
        dependentService.BaseService.ShouldNotBeNull();
    }

    [Fact]
    public void PropertyInjection_ShouldSupportOptionalDependencies()
    {
        // Arrange & Act
        var host = ApplicationBuilder.Create()
            .UseRootModule<TestModuleWithOptionalDependencies>()
            .Build();

        var service = host.Services.GetRequiredService<IServiceWithOptionalDependency>();

        // Assert
        service.ShouldBeOfType<ServiceWithOptionalDependency>();
        var serviceImpl = (ServiceWithOptionalDependency)service;
        serviceImpl.RequiredDependency.ShouldNotBeNull();
        serviceImpl.OptionalDependency.ShouldBeNull(); // Not registered
    }

    [Fact]
    public void PropertyInjection_ShouldWorkWithScopedServices()
    {
        // Arrange
        var host = ApplicationBuilder.Create()
            .UseRootModule<TestModuleWithScopedServices>()
            .Build();

        // Act
        IScopedService? service1, service2;
        using (var scope1 = host.Services.CreateScope())
        {
            service1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
        }
        using (var scope2 = host.Services.CreateScope())
        {
            service2 = scope2.ServiceProvider.GetRequiredService<IScopedService>();
        }

        // Assert
        service1.ShouldNotBeNull();
        service2.ShouldNotBeNull();
        service1.ShouldNotBeSameAs(service2);
    }

    // Test modules
    [DependsOn(typeof(EssentialsCoreModule))]
    public class TestModule : EssentialsModule
    {
    }

    [DependsOn(typeof(EssentialsCoreModule))]
    public class TestModuleWithPropertyInjection : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            context.Services.AddSingleton<ILogger, ConsoleLogger>();
            context.Services.AddSingleton<IRepository, Repository>();
            context.Services.AddSingleton<TestServiceWithInjectedProperty>();
            context.Services.AddSingletonWithPropertyInjection<IComplexService, ComplexService>();
        }

        public override void Initialize(ModuleContext context)
        {
            var service = context.GetRequiredService<TestServiceWithInjectedProperty>();
            context.InjectProperties(service);
        }
    }

    [DependsOn(typeof(EssentialsCoreModule))]
    public class BaseModule : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            context.Services.AddSingleton<IBaseService, BaseServiceImpl>();
        }
    }

    [DependsOn(typeof(BaseModule))]
    public class DependentModule : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            context.Services.AddSingletonWithPropertyInjection<IDependentService, DependentServiceImpl>();
        }
    }

    [DependsOn(typeof(EssentialsCoreModule))]
    public class TestModuleWithOptionalDependencies : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            context.Services.AddSingleton<IRequiredDependency, RequiredDependency>();
            // OptionalDependency is NOT registered
            context.Services.AddSingletonWithPropertyInjection<IServiceWithOptionalDependency, ServiceWithOptionalDependency>();
        }
    }

    [DependsOn(typeof(EssentialsCoreModule))]
    public class TestModuleWithScopedServices : EssentialsModule
    {
        public override void ConfigureServices(ModuleContext context)
        {
            context.Services.AddScoped<IScopedDependency, ScopedDependency>();
            context.Services.AddScopedWithPropertyInjection<IScopedService, ScopedServiceImpl>();
        }
    }

    // Test interfaces and implementations
    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine(message);
    }

    public interface IRepository { }
    public class Repository : IRepository { }

    public class TestServiceWithInjectedProperty
    {
        [InjectProperty]
        public ILogger Logger { get; set; } = null!;
    }

    public interface IComplexService { }

    public class ComplexService : IComplexService
    {
        [InjectProperty]
        public ILogger Logger { get; set; } = null!;

        [InjectProperty]
        public IRepository Repository { get; set; } = null!;
    }

    public interface IBaseService { }
    public class BaseServiceImpl : IBaseService { }

    public interface IDependentService { }

    public class DependentServiceImpl : IDependentService
    {
        [InjectProperty]
        public IBaseService BaseService { get; set; } = null!;
    }

    public interface IRequiredDependency { }
    public class RequiredDependency : IRequiredDependency { }

    public interface IOptionalDependency { }

    public interface IServiceWithOptionalDependency { }

    public class ServiceWithOptionalDependency : IServiceWithOptionalDependency
    {
        [InjectProperty]
        public IRequiredDependency RequiredDependency { get; set; } = null!;

        [InjectProperty(Required = false)]
        public IOptionalDependency? OptionalDependency { get; set; }
    }

    public interface IScopedDependency { }
    public class ScopedDependency : IScopedDependency { }

    public interface IScopedService { }

    public class ScopedServiceImpl : IScopedService
    {
        [InjectProperty]
        public IScopedDependency Dependency { get; set; } = null!;
    }
}
