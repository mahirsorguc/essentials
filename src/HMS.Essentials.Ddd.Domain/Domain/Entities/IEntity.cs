namespace HMS.Essentials.Domain.Entities;

/// <summary>
/// Defines a base interface for all entities.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the primary keys of the entity as an array of objects.
    /// </summary>
    /// <returns>An array of primary key values.</returns>
    object[] GetKeys();
}

/// <summary>
/// Defines a base interface for entities with a single primary key.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    TKey Id { get; }
}
