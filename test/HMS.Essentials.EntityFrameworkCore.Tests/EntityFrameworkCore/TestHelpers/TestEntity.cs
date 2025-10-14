using HMS.Essentials.Domain.Entities;

namespace HMS.Essentials.EntityFrameworkCore.TestHelpers;

/// <summary>
/// Test entity for unit tests.
/// </summary>
public class TestEntity : IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public object[] GetKeys()
    {
        return new object[] { Id };
    }
}

/// <summary>
/// Test entity with Guid key for unit tests.
/// </summary>
public class TestEntityWithGuid : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public object[] GetKeys()
    {
        return new object[] { Id };
    }
}

/// <summary>
/// Test entity with string key for unit tests.
/// </summary>
public class TestEntityWithStringKey : IEntity<string>
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public object[] GetKeys()
    {
        return new object[] { Id };
    }
}
