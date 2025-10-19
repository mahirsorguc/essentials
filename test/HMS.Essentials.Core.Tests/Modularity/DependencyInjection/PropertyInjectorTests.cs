using HMS.Essentials.Modularity.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.Modularity.Tests.DependencyInjection;

/// <summary>
/// Tests for <see cref="PropertyInjector"/>.
/// </summary>
public class PropertyInjectorTests
{
    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new PropertyInjector(null!));
    }

    [Fact]
    public void InjectProperties_WithNullInstance_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => injector.InjectProperties(null!));
    }

    [Fact]
    public void InjectProperties_ShouldInjectRequiredProperty()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithRequiredProperty();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.TestService.ShouldNotBeNull();
        instance.TestService.ShouldBeOfType<TestService>();
    }

    [Fact]
    public void InjectProperties_ShouldInjectOptionalProperty_WhenServiceExists()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithOptionalProperty();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.TestService.ShouldNotBeNull();
        instance.TestService.ShouldBeOfType<TestService>();
    }

    [Fact]
    public void InjectProperties_ShouldNotInjectOptionalProperty_WhenServiceDoesNotExist()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithOptionalProperty();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.TestService.ShouldBeNull();
    }

    [Fact]
    public void InjectProperties_ShouldThrowException_WhenRequiredPropertyCannotBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithRequiredProperty();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => injector.InjectProperties(instance))
            .Message.ShouldContain("Required property");
    }

    [Fact]
    public void InjectProperties_ShouldInjectMultipleProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        services.AddSingleton<IAnotherService, AnotherService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithMultipleProperties();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.TestService.ShouldNotBeNull();
        instance.AnotherService.ShouldNotBeNull();
        instance.TestService.ShouldBeOfType<TestService>();
        instance.AnotherService.ShouldBeOfType<AnotherService>();
    }

    [Fact]
    public void InjectProperties_ShouldNotInjectPropertiesWithoutAttribute()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithNonInjectedProperty();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.NonInjectedService.ShouldBeNull();
    }

    [Fact]
    public void InjectProperties_ShouldThrowException_WhenPropertyIsNotSettable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithReadOnlyProperty();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => injector.InjectProperties(instance))
            .Message.ShouldContain("not settable");
    }

    [Fact]
    public void InjectProperties_Generic_ShouldInjectProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithRequiredProperty();

        // Act
        injector.InjectProperties<TestClassWithRequiredProperty>(instance);

        // Assert
        instance.TestService.ShouldNotBeNull();
    }

    [Fact]
    public void InjectProperties_ShouldWorkWithPrivateSetters()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new TestClassWithPrivateSetter();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.GetTestService().ShouldNotBeNull();
    }

    [Fact]
    public void InjectProperties_ShouldCachePropertyInfo()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance1 = new TestClassWithRequiredProperty();
        var instance2 = new TestClassWithRequiredProperty();

        // Act
        injector.InjectProperties(instance1);
        injector.InjectProperties(instance2);

        // Assert
        instance1.TestService.ShouldNotBeNull();
        instance2.TestService.ShouldNotBeNull();
    }

    [Fact]
    public void InjectProperties_ShouldWorkWithInheritedProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        services.AddSingleton<IAnotherService, AnotherService>();
        var serviceProvider = services.BuildServiceProvider();
        var injector = new PropertyInjector(serviceProvider);
        var instance = new DerivedTestClass();

        // Act
        injector.InjectProperties(instance);

        // Assert
        instance.TestService.ShouldNotBeNull();
        instance.AnotherService.ShouldNotBeNull();
    }

    // Test helper classes
    public interface ITestService { }
    public class TestService : ITestService { }

    public interface IAnotherService { }
    public class AnotherService : IAnotherService { }

    public class TestClassWithRequiredProperty
    {
        [InjectProperty]
        public ITestService TestService { get; set; } = null!;
    }

    public class TestClassWithOptionalProperty
    {
        [InjectProperty(Required = false)]
        public ITestService? TestService { get; set; }
    }

    public class TestClassWithMultipleProperties
    {
        [InjectProperty]
        public ITestService TestService { get; set; } = null!;

        [InjectProperty]
        public IAnotherService AnotherService { get; set; } = null!;
    }

    public class TestClassWithNonInjectedProperty
    {
        public ITestService? NonInjectedService { get; set; }
    }

    public class TestClassWithReadOnlyProperty
    {
        [InjectProperty]
        public ITestService TestService { get; } = null!;
    }

    public class TestClassWithPrivateSetter
    {
        [InjectProperty]
        public ITestService TestService { get; private set; } = null!;

        public ITestService GetTestService() => TestService;
    }

    public class BaseTestClass
    {
        [InjectProperty]
        public ITestService TestService { get; set; } = null!;
    }

    public class DerivedTestClass : BaseTestClass
    {
        [InjectProperty]
        public IAnotherService AnotherService { get; set; } = null!;
    }
}
