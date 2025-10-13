namespace HMS.Essentials.MediatR;

/// <summary>
/// Handler for domain events.
/// Multiple handlers can handle the same domain event.
/// Implements MediatR's INotificationHandler interface.
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event to handle.</typeparam>
public interface IDomainEventHandler<in TDomainEvent> : global::MediatR.INotificationHandler<TDomainEvent>
    where TDomainEvent : global::MediatR.INotification
{
}
