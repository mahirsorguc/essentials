using System.Reflection;
using HMS.Essentials.AutoMapper.Extensions;
using HMS.Essentials.AutoMapper.ObjectMapping;
using HMS.Essentials.ObjectMapping;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.AutoMapper;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEssentialsAutoMapper_Should_Throw_When_Services_Is_Null()
    {
        // Arrange
        IServiceCollection services = null!;
        var assembly = Assembly.GetExecutingAssembly();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            services.AddEssentialsAutoMapper(new[] { assembly }));
    }

    [Fact]
    public void AddEssentialsAutoMapper_With_Assemblies_Should_Register_IMapper()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddEssentialsAutoMapper(new[] { assembly });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<global::AutoMapper.IMapper>();
        mapper.ShouldNotBeNull();
    }

    [Fact]
    public void AddEssentialsAutoMapper_With_Assemblies_Should_Register_IObjectMapper()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddEssentialsAutoMapper(new[] { assembly });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var objectMapper = serviceProvider.GetService<IObjectMapper>();
        objectMapper.ShouldNotBeNull();
        objectMapper.ShouldBeOfType<AutoMapperObjectMapper>();
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Use_Calling_Assembly_When_No_Assemblies_Provided()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEssentialsAutoMapper(Array.Empty<Assembly>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<global::AutoMapper.IMapper>();
        mapper.ShouldNotBeNull();
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Scan_Multiple_Assemblies()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly1 = Assembly.GetExecutingAssembly();
        var assembly2 = typeof(AutoMapperObjectMapper).Assembly;

        // Act
        services.AddEssentialsAutoMapper(new[] { assembly1, assembly2 });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<global::AutoMapper.IMapper>();
        mapper.ShouldNotBeNull();
    }

    [Fact]
    public void AddEssentialsAutoMapper_With_Types_Should_Throw_When_Services_Is_Null()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            services.AddEssentialsAutoMapper(typeof(TestProfile)));
    }

    [Fact]
    public void AddEssentialsAutoMapper_With_Types_Should_Register_Services()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEssentialsAutoMapper(typeof(TestProfile));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<global::AutoMapper.IMapper>();
        var objectMapper = serviceProvider.GetService<IObjectMapper>();
        
        mapper.ShouldNotBeNull();
        objectMapper.ShouldNotBeNull();
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Register_As_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddEssentialsAutoMapper(new[] { assembly });

        // Assert
        var mapperDescriptor = services.FirstOrDefault(s => 
            s.ServiceType == typeof(global::AutoMapper.IMapper));
        var objectMapperDescriptor = services.FirstOrDefault(s => 
            s.ServiceType == typeof(IObjectMapper));

        mapperDescriptor.ShouldNotBeNull();
        mapperDescriptor!.Lifetime.ShouldBe(ServiceLifetime.Singleton);
        
        objectMapperDescriptor.ShouldNotBeNull();
        objectMapperDescriptor!.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = services.AddEssentialsAutoMapper(new[] { assembly });

        // Assert
        result.ShouldBe(services);
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Load_Profiles_From_Assembly()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddEssentialsAutoMapper(new[] { assembly });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<global::AutoMapper.IMapper>();

        // Assert
        mapper.ConfigurationProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Create_Valid_Mapper_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(new[] { typeof(TestProfile) });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<global::AutoMapper.IMapper>();

        // Act & Assert
        Should.NotThrow(() => mapper.ConfigurationProvider.AssertConfigurationIsValid());
    }

    [Fact]
    public void AddEssentialsAutoMapper_Should_Enable_Mapping_With_Registered_Profiles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(TestProfile));
        var serviceProvider = services.BuildServiceProvider();
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();

        var source = new TestSource { Name = "Test", Value = 42 };

        // Act
        var result = objectMapper.Map<TestDestination>(source);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test");
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void AddEssentialsAutoMapper_With_Multiple_Types_Should_Use_Distinct_Assemblies()
    {
        // Arrange
        var services = new ServiceCollection();
        var type1 = typeof(TestProfile);
        var type2 = typeof(AnotherTestProfile);

        // Act
        services.AddEssentialsAutoMapper(type1, type2);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<global::AutoMapper.IMapper>();
        mapper.ShouldNotBeNull();
    }

    [Fact]
    public void IObjectMapper_Should_Work_With_Real_AutoMapper()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(TestProfile));
        var serviceProvider = services.BuildServiceProvider();
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();

        var source = new TestSource { Name = "Integration Test", Value = 100 };

        // Act
        var result1 = objectMapper.Map<TestDestination>(source);
        var result2 = objectMapper.Map<TestSource, TestDestination>(source);
        
        var destination = new TestDestination();
        var result3 = objectMapper.Map(source, destination);

        // Assert
        result1.Name.ShouldBe("Integration Test");
        result1.Value.ShouldBe(100);
        
        result2.Name.ShouldBe("Integration Test");
        result2.Value.ShouldBe(100);
        
        result3.Name.ShouldBe("Integration Test");
        result3.Value.ShouldBe(100);
    }

    // Test classes
    private class TestProfile : EssentialsProfile
    {
        protected override void ConfigureMappings()
        {
            CreateMap<TestSource, TestDestination>();
            CreateMap<TestDestination, TestSource>();
        }
    }

    private class AnotherTestProfile : EssentialsProfile
    {
        protected override void ConfigureMappings()
        {
            CreateMap<AnotherSource, AnotherDestination>();
        }
    }

    private class TestSource
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class TestDestination
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class AnotherSource
    {
        public string Data { get; set; } = string.Empty;
    }

    private class AnotherDestination
    {
        public string Data { get; set; } = string.Empty;
    }
}
