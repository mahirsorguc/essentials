using HMS.Essentials.MediatR;

namespace HMS.Essentials.Domain.Entities;

/// <summary>
/// Base class for aggregate roots.
/// Aggregate roots are the main entities that represent consistency boundaries in the domain.
/// They encapsulate domain events and maintain invariants.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the domain events that have been raised by this aggregate root.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected virtual void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    protected virtual void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the aggregate root.
    /// </summary>
    public virtual void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Base class for aggregate roots with an integer primary key.
/// </summary>
public abstract class AggregateRoot : AggregateRoot<int>
{
}

/// <summary>
/// Base class for aggregate roots with a GUID primary key.
/// </summary>
public abstract class GuidAggregateRoot : AggregateRoot<Guid>
{
    protected GuidAggregateRoot()
    {
        Id = Guid.NewGuid();
    }
}
