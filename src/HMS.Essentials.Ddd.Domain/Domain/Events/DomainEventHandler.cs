namespace HMS.Essentials.Domain.Events;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    public abstract Task Handle(TDomainEvent notification, CancellationToken cancellationToken);
}