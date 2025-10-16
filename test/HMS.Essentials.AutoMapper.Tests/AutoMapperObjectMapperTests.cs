using AutoMapper;
using HMS.Essentials.AutoMapper.ObjectMapping;
using Moq;
using Shouldly;

namespace HMS.Essentials.AutoMapper;

/// <summary>
/// Tests for <see cref="AutoMapperObjectMapper"/>.
/// </summary>
public class AutoMapperObjectMapperTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly AutoMapperObjectMapper _objectMapper;

    public AutoMapperObjectMapperTests()
    {
        _mapperMock = new Mock<IMapper>();
        _objectMapper = new AutoMapperObjectMapper(_mapperMock.Object);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Mapper_Is_Null()
    {
        // Arrange, Act & Assert
        Should.Throw<ArgumentNullException>(() => new AutoMapperObjectMapper(null!));
    }

    [Fact]
    public void Constructor_Should_Create_Instance_When_Mapper_Is_Valid()
    {
        // Arrange & Act
        var mapper = new AutoMapperObjectMapper(_mapperMock.Object);

        // Assert
        mapper.ShouldNotBeNull();
    }

    [Fact]
    public void Map_With_Single_Generic_Should_Call_Mapper()
    {
        // Arrange
        var source = new SourceClass { Name = "Test" };
        var expected = new DestinationClass { Title = "Test" };

        _mapperMock
            .Setup(m => m.Map<DestinationClass>(source))
            .Returns(expected);

        // Act
        var result = _objectMapper.Map<DestinationClass>(source);

        // Assert
        result.ShouldBe(expected);
        _mapperMock.Verify(m => m.Map<DestinationClass>(source), Times.Once);
    }

    [Fact]
    public void Map_With_Two_Generics_Should_Call_Mapper()
    {
        // Arrange
        var source = new SourceClass { Name = "Test" };
        var expected = new DestinationClass { Title = "Test" };

        _mapperMock
            .Setup(m => m.Map<SourceClass, DestinationClass>(source))
            .Returns(expected);

        // Act
        var result = _objectMapper.Map<SourceClass, DestinationClass>(source);

        // Assert
        result.ShouldBe(expected);
        _mapperMock.Verify(m => m.Map<SourceClass, DestinationClass>(source), Times.Once);
    }

    [Fact]
    public void Map_With_Existing_Destination_Should_Call_Mapper()
    {
        // Arrange
        var source = new SourceClass { Name = "Updated" };
        var destination = new DestinationClass { Title = "Original" };
        var expected = new DestinationClass { Title = "Updated" };

        _mapperMock
            .Setup(m => m.Map(source, destination))
            .Returns(expected);

        // Act
        var result = _objectMapper.Map(source, destination);

        // Assert
        result.ShouldBe(expected);
        _mapperMock.Verify(m => m.Map(source, destination), Times.Once);
    }

    [Fact]
    public void Map_Should_Return_Null_When_Source_Is_Null()
    {
        // Arrange
        SourceClass? source = null;

        _mapperMock
            .Setup(m => m.Map<DestinationClass>(source))
            .Returns((DestinationClass)null!);

        // Act
        var result = _objectMapper.Map<DestinationClass>(source!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Map_With_Two_Generics_Should_Return_Null_When_Source_Is_Null()
    {
        // Arrange
        SourceClass? source = null;

        _mapperMock
            .Setup(m => m.Map<SourceClass, DestinationClass>(source!))
            .Returns((DestinationClass)null!);

        // Act
        var result = _objectMapper.Map<SourceClass, DestinationClass>(source!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Map_Should_Handle_Complex_Objects()
    {
        // Arrange
        var source = new ComplexSource
        {
            Id = 1,
            Name = "Complex",
            NestedObject = new NestedSource { Value = "Nested" }
        };

        var expected = new ComplexDestination
        {
            Id = 1,
            Name = "Complex",
            NestedObject = new NestedDestination { Value = "Nested" }
        };

        _mapperMock
            .Setup(m => m.Map<ComplexDestination>(source))
            .Returns(expected);

        // Act
        var result = _objectMapper.Map<ComplexDestination>(source);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(expected.Id);
        result.Name.ShouldBe(expected.Name);
        result.NestedObject.ShouldNotBeNull();
        result.NestedObject.Value.ShouldBe(expected.NestedObject.Value);
    }

    [Fact]
    public void Map_Should_Handle_Collections()
    {
        // Arrange
        var sourceList = new List<SourceClass>
        {
            new() { Name = "Item1" },
            new() { Name = "Item2" }
        };

        var expectedList = new List<DestinationClass>
        {
            new() { Title = "Item1" },
            new() { Title = "Item2" }
        };

        _mapperMock
            .Setup(m => m.Map<List<DestinationClass>>(sourceList))
            .Returns(expectedList);

        // Act
        var result = _objectMapper.Map<List<DestinationClass>>(sourceList);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    // Test DTOs
    private class SourceClass
    {
        public string Name { get; set; } = string.Empty;
    }

    private class DestinationClass
    {
        public string Title { get; set; } = string.Empty;
    }

    private class ComplexSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NestedSource NestedObject { get; set; } = null!;
    }

    private class ComplexDestination
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NestedDestination NestedObject { get; set; } = null!;
    }

    private class NestedSource
    {
        public string Value { get; set; } = string.Empty;
    }

    private class NestedDestination
    {
        public string Value { get; set; } = string.Empty;
    }
}
