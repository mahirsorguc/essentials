using HMS.Essentials.Modularity;
using HMS.Essentials.Modularity.DependencyInjection;
using HMS.Essentials.SequentialGuid;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace HMS.Essentials.Core.Tests.Modularity.DependencyInjection;

/// <summary>
/// Tests for <see cref="IEssentialsLazyServiceProvider"/> and <see cref="EssentialsLazyServiceProvider"/>.
/// </summary>
public class EssentialsLazyServiceProviderTests
{
    #region Test Services

    public interface ITestService
    {
        string GetMessage();
    }

    public class TestService : ITestService
    {
        public string GetMessage() => "Test Service";
    }

    public interface IComplexService
    {
        ITestService TestService { get; }
    }

    public class ComplexService : IComplexService
    {
        public ITestService TestService { get; }

        public ComplexService(ITestService testService)
        {
            TestService = testService;
        }
    }

    #endregion

    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new EssentialsLazyServiceProvider(null!));
    }

    [Fact]
    public void LazyGetRequiredService_Generic_ShouldReturnLazyInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetRequiredService<ITestService>();

        // Assert
        lazyService.ShouldNotBeNull();
        lazyService.ShouldBeOfType<Lazy<ITestService>>();
        lazyService.IsValueCreated.ShouldBeFalse(); // Service not yet resolved
    }

    [Fact]
    public void LazyGetRequiredService_Generic_WhenAccessed_ShouldResolveService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetRequiredService<ITestService>();
        var service = lazyService.Value; // Access the value

        // Assert
        lazyService.IsValueCreated.ShouldBeTrue();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestService>();
        service.GetMessage().ShouldBe("Test Service");
    }

    [Fact]
    public void LazyGetRequiredService_Generic_WithUnregisteredService_ShouldThrowWhenAccessed()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetRequiredService<ITestService>();

        // Assert
        lazyService.ShouldNotBeNull();
        Should.Throw<InvalidOperationException>(() => _ = lazyService.Value);
    }

    [Fact]
    public void LazyGetService_Generic_ShouldReturnLazyInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetService<ITestService>();

        // Assert
        lazyService.ShouldNotBeNull();
        lazyService.ShouldBeOfType<Lazy<ITestService>>();
        lazyService.IsValueCreated.ShouldBeFalse();
    }

    [Fact]
    public void LazyGetService_Generic_WhenAccessed_ShouldResolveService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetService<ITestService>();
        var service = lazyService!.Value;

        // Assert
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestService>();
    }

    [Fact]
    public void LazyGetRequiredService_NonGeneric_ShouldReturnLazyInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetRequiredService(typeof(ITestService));

        // Assert
        lazyService.ShouldNotBeNull();
        lazyService.ShouldBeOfType<Lazy<object>>();
        lazyService.IsValueCreated.ShouldBeFalse();
    }

    [Fact]
    public void LazyGetRequiredService_NonGeneric_WhenAccessed_ShouldResolveService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetRequiredService(typeof(ITestService));
        var service = lazyService.Value;

        // Assert
        lazyService.IsValueCreated.ShouldBeTrue();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestService>();
    }

    [Fact]
    public void LazyGetRequiredService_NonGeneric_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => lazyProvider.LazyGetRequiredService(null!));
    }

    [Fact]
    public void LazyGetService_NonGeneric_ShouldReturnLazyInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetService(typeof(ITestService));

        // Assert
        lazyService.ShouldNotBeNull();
        lazyService.ShouldBeOfType<Lazy<object?>>();
        lazyService.IsValueCreated.ShouldBeFalse();
    }

    [Fact]
    public void LazyGetService_NonGeneric_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => lazyProvider.LazyGetService(null!));
    }

    [Fact]
    public void LazyGetService_NonGeneric_WhenAccessed_ShouldResolveService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetService(typeof(ITestService));
        var service = lazyService!.Value;

        // Assert
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TestService>();
    }

    [Fact]
    public void LazyGetService_NonGeneric_WithUnregisteredService_ShouldReturnNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetService(typeof(ITestService));
        var service = lazyService!.Value;

        // Assert
        service.ShouldBeNull();
    }

    [Fact]
    public void MultipleLazyServices_ShouldNotInterfereWithEachOther()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        services.AddSingleton<IComplexService, ComplexService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyTestService = lazyProvider.LazyGetRequiredService<ITestService>();
        var lazyComplexService = lazyProvider.LazyGetRequiredService<IComplexService>();

        // Assert
        lazyTestService.IsValueCreated.ShouldBeFalse();
        lazyComplexService.IsValueCreated.ShouldBeFalse();

        var testService = lazyTestService.Value;
        lazyTestService.IsValueCreated.ShouldBeTrue();
        lazyComplexService.IsValueCreated.ShouldBeFalse(); // Still not resolved

        var complexService = lazyComplexService.Value;
        lazyComplexService.IsValueCreated.ShouldBeTrue();

        testService.ShouldNotBeNull();
        complexService.ShouldNotBeNull();
    }

    [Fact]
    public void LazyService_WithScopedDependency_ShouldWorkCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act & Assert
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedLazyProvider = new EssentialsLazyServiceProvider(scope.ServiceProvider);
            var lazyService = scopedLazyProvider.LazyGetRequiredService<ITestService>();
            
            lazyService.IsValueCreated.ShouldBeFalse();
            var service = lazyService.Value;
            lazyService.IsValueCreated.ShouldBeTrue();
            service.ShouldNotBeNull();
        }
    }

    [Fact]
    public void LazyService_ShouldCacheResolvedInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var serviceProvider = services.BuildServiceProvider();
        var lazyProvider = new EssentialsLazyServiceProvider(serviceProvider);

        // Act
        var lazyService = lazyProvider.LazyGetRequiredService<ITestService>();
        var service1 = lazyService.Value;
        var service2 = lazyService.Value;

        // Assert
        service1.ShouldBeSameAs(service2); // Lazy caches the instance
    }

    [Fact]
    public void LazyProvider_ShouldBeRegisteredInCoreModule()
    {
        // Arrange
        var host = ApplicationBuilder.Create()
            .UseRootModule<EssentialsCoreModule>()
            .Build();

        // Act
        var lazyProvider = host.Services.GetService<IEssentialsLazyServiceProvider>();

        // Assert
        lazyProvider.ShouldNotBeNull();
        lazyProvider.ShouldBeOfType<EssentialsLazyServiceProvider>();
    }

    [Fact]
    public void LazyProvider_FromModuleContext_ShouldWork()
    {
        // Arrange
        var host = ApplicationBuilder.Create()
            .UseRootModule<EssentialsCoreModule>()
            .Build();

        // Act
        var lazyProvider = host.Services.GetRequiredService<IEssentialsLazyServiceProvider>();
        var lazyGuidGenerator = lazyProvider.LazyGetRequiredService<ISequentialGuidGenerator>();

        // Assert
        lazyGuidGenerator.ShouldNotBeNull();
        lazyGuidGenerator.IsValueCreated.ShouldBeFalse();
        
        var guidGenerator = lazyGuidGenerator.Value;
        lazyGuidGenerator.IsValueCreated.ShouldBeTrue();
        guidGenerator.ShouldNotBeNull();
    }
}
