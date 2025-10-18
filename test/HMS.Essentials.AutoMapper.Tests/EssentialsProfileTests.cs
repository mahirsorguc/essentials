using AutoMapper;
using HMS.Essentials.AutoMapper.ObjectMapping;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace HMS.Essentials.AutoMapper;

/// <summary>
/// Tests for <see cref="EssentialsProfile"/>.
/// </summary>
public class EssentialsProfileTests
{
    [Fact]
    public void Profile_Should_Be_Abstract()
    {
        // Arrange & Act
        var profileType = typeof(EssentialsProfile);

        // Assert
        profileType.IsAbstract.ShouldBeTrue();
    }

    [Fact]
    public void Profile_Should_Inherit_From_AutoMapper_Profile()
    {
        // Arrange & Act
        var profileType = typeof(EssentialsProfile);

        // Assert
        profileType.BaseType.ShouldBe(typeof(global::AutoMapper.Profile));
    }

    [Fact]
    public void Profile_Should_Have_ConfigureMappings_Method()
    {
        // Arrange & Act
        var method = typeof(EssentialsProfile).GetMethod(
            "ConfigureMappings",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Assert
        method.ShouldNotBeNull();
        method!.IsAbstract.ShouldBeTrue();
    }

    [Fact]
    public void Concrete_Profile_Should_Be_Instantiable()
    {
        // Arrange & Act
        var profile = new TestProfile();

        // Assert
        profile.ShouldNotBeNull();
        profile.ShouldBeOfType<TestProfile>();
    }

    [Fact]
    public void Concrete_Profile_Should_Call_ConfigureMappings()
    {
        // Arrange & Act
        var profile = new TestProfile();

        // Assert
        profile.ConfigureMappingsCalled.ShouldBeTrue();
    }

    [Fact]
    public void Profile_Should_Support_Mapping_Configuration()
    {
        // Arrange
        var profile = new TestProfileWithMappings();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(profile);
        }, NullLoggerFactory.Instance);

        // Act
        var mapper = config.CreateMapper();
        var source = new TestSource { Name = "Test" };
        var result = mapper.Map<TestDestination>(source);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test");
    }

    // Test implementation
    private class TestProfile : EssentialsProfile
    {
        public bool ConfigureMappingsCalled { get; private set; }

        protected override void ConfigureMappings()
        {
            ConfigureMappingsCalled = true;
        }
    }

    private class TestProfileWithMappings : EssentialsProfile
    {
        protected override void ConfigureMappings()
        {
            CreateMap<TestSource, TestDestination>();
        }
    }

    private class TestSource
    {
        public string Name { get; set; } = string.Empty;
    }

    private class TestDestination
    {
        public string Name { get; set; } = string.Empty;
    }
}
