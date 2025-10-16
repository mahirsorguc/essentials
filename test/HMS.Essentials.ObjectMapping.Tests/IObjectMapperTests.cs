using Shouldly;

namespace HMS.Essentials.ObjectMapping;

/// <summary>
/// Tests for <see cref="IObjectMapper"/> interface.
/// </summary>
public class IObjectMapperTests
{
    [Fact]
    public void Interface_Should_Have_Map_Method_With_Single_Generic()
    {
        // Arrange
        var method = typeof(IObjectMapper).GetMethod(
            nameof(IObjectMapper.Map), 
            1,
            new[] { typeof(object) });

        // Assert
        method.ShouldNotBeNull();
        method!.IsGenericMethod.ShouldBeTrue();
        method.GetGenericArguments().Length.ShouldBe(1);
    }

    [Fact]
    public void Interface_Should_Have_Map_Method_With_Two_Generics()
    {
        // Arrange
        var methods = typeof(IObjectMapper).GetMethods()
            .Where(m => m.Name == nameof(IObjectMapper.Map) && m.GetGenericArguments().Length == 2)
            .ToList();

        // Assert
        methods.ShouldNotBeEmpty();
        methods.Count.ShouldBe(2); // One for new destination, one for existing destination
    }

    [Fact]
    public void Interface_Should_Have_Map_Method_With_Existing_Destination()
    {
        // Arrange
        var method = typeof(IObjectMapper).GetMethods()
            .FirstOrDefault(m => 
                m.Name == nameof(IObjectMapper.Map) && 
                m.GetGenericArguments().Length == 2 &&
                m.GetParameters().Length == 2);

        // Assert
        method.ShouldNotBeNull();
        method!.IsGenericMethod.ShouldBeTrue();
    }

    [Fact]
    public void Interface_Should_Be_Public()
    {
        // Arrange & Act
        var type = typeof(IObjectMapper);

        // Assert
        type.IsPublic.ShouldBeTrue();
        type.IsInterface.ShouldBeTrue();
    }
}
