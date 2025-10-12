using Shouldly;

namespace HMS.Essentials.Domain.Entities;

public class EntityTests
{
    private class TestEntity : Entity<int>
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void Entity_WithIntKey_ShouldInitialize()
    {
        // Arrange & Act
        var entity = new TestEntity { Id = 1, Name = "Test" };

        // Assert
        entity.Id.ShouldBe(1);
        entity.Name.ShouldBe("Test");
    }

    [Fact]
    public void Entity_WithDefaultId_ShouldBeTransient()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.IsTransient().ShouldBeTrue();
    }

    [Fact]
    public void Entity_WithNonDefaultId_ShouldNotBeTransient()
    {
        // Arrange & Act
        var entity = new TestEntity { Id = 1 };

        // Assert
        entity.IsTransient().ShouldBeFalse();
    }
}
