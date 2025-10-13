using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Marker interface for domain events.
/// Domain events are notifications about something that has happened in the domain.
/// </summary>
public interface IDomainEvent : INotification
{
}