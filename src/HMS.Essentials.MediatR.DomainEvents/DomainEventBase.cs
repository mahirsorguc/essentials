using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract record DomainEventBase : IDomainEvent, INotification
{
    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the unique identifier for this event.
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();
}
