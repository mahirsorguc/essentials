namespace HMS.Essentials.Domain.Entities;

/// <summary>
/// Defines an interface for aggregate roots.
/// Aggregate roots are entities that serve as entry points to aggregates.
/// </summary>
public interface IAggregateRoot : IEntity
{
}

/// <summary>
/// Defines an interface for aggregate roots with a single primary key.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
{
}
