using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity.DependencyInjection;

/// <summary>
/// Tests for property injection extension methods in <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public class PropertyInjectionExtensionsTests
{
    [Fact]
    public void AddSingletonWithPropertyInjection_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddSingleton<IDependencyService, DependencyService>();

        // Act
        services.AddSingletonWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITestService>();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestServiceWithPropertyInjection>();
    }

    [Fact]
    public void AddSingletonWithPropertyInjection_ShouldInjectProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddSingleton<IDependencyService, DependencyService>();

        // Act
        services.AddSingletonWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITestService>() as TestServiceWithPropertyInjection;

        // Assert
        service.ShouldNotBeNull();
        service.Dependency.ShouldNotBeNull();
        service.Dependency.ShouldBeOfType<DependencyService>();
    }

    [Fact]
    public void AddScopedWithPropertyInjection_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddScoped<IDependencyService, DependencyService>();

        // Act
        services.AddScopedWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITestService>();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestServiceWithPropertyInjection>();
    }

    [Fact]
    public void AddScopedWithPropertyInjection_ShouldInjectProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddScoped<IDependencyService, DependencyService>();

        // Act
        services.AddScopedWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITestService>() as TestServiceWithPropertyInjection;
        service.ShouldNotBeNull();
        service.Dependency.ShouldNotBeNull();
    }

    [Fact]
    public void AddScopedWithPropertyInjection_ShouldCreateNewInstancePerScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddScoped<IDependencyService, DependencyService>();
        services.AddScopedWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        ITestService? service1, service2;
        using (var scope1 = serviceProvider.CreateScope())
        {
            service1 = scope1.ServiceProvider.GetService<ITestService>();
        }
        using (var scope2 = serviceProvider.CreateScope())
        {
            service2 = scope2.ServiceProvider.GetService<ITestService>();
        }

        // Assert
        service1.ShouldNotBeNull();
        service2.ShouldNotBeNull();
        service1.ShouldNotBeSameAs(service2);
    }

    [Fact]
    public void AddTransientWithPropertyInjection_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddTransient<IDependencyService, DependencyService>();

        // Act
        services.AddTransientWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITestService>();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestServiceWithPropertyInjection>();
    }

    [Fact]
    public void AddTransientWithPropertyInjection_ShouldInjectProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddTransient<IDependencyService, DependencyService>();

        // Act
        services.AddTransientWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITestService>() as TestServiceWithPropertyInjection;

        // Assert
        service.ShouldNotBeNull();
        service.Dependency.ShouldNotBeNull();
    }

    [Fact]
    public void AddTransientWithPropertyInjection_ShouldCreateNewInstanceEachTime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddTransient<IDependencyService, DependencyService>();
        services.AddTransientWithPropertyInjection<ITestService, TestServiceWithPropertyInjection>();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service1 = serviceProvider.GetService<ITestService>();
        var service2 = serviceProvider.GetService<ITestService>();

        // Assert
        service1.ShouldNotBeNull();
        service2.ShouldNotBeNull();
        service1.ShouldNotBeSameAs(service2);
    }

    [Fact]
    public void PropertyInjection_ShouldWorkWithConstructorInjection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPropertyInjector, PropertyInjector>();
        services.AddSingleton<IDependencyService, DependencyService>();
        services.AddSingleton<IAnotherDependency, AnotherDependency>();

        // Act
        services.AddSingletonWithPropertyInjection<ITestService, TestServiceWithBothInjectionTypes>();
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITestService>() as TestServiceWithBothInjectionTypes;

        // Assert
        service.ShouldNotBeNull();
        service.ConstructorDependency.ShouldNotBeNull();
        service.PropertyDependency.ShouldNotBeNull();
    }

    // Test helper classes and interfaces
    public interface ITestService { }
    public interface IDependencyService { }
    public interface IAnotherDependency { }

    public class DependencyService : IDependencyService { }
    public class AnotherDependency : IAnotherDependency { }

    public class TestServiceWithPropertyInjection : ITestService
    {
        [InjectProperty]
        public IDependencyService Dependency { get; set; } = null!;
    }

    public class TestServiceWithBothInjectionTypes : ITestService
    {
        public TestServiceWithBothInjectionTypes(IAnotherDependency constructorDependency)
        {
            ConstructorDependency = constructorDependency;
        }

        public IAnotherDependency ConstructorDependency { get; }

        [InjectProperty]
        public IDependencyService PropertyDependency { get; set; } = null!;
    }
}
