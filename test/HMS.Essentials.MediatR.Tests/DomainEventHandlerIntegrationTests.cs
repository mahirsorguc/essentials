using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

public class DomainEventHandlerIntegrationTests
{
    [Fact]
    public void IDomainEventHandler_ShouldImplementMediatRInterface()
    {
        // Assert
        typeof(INotificationHandler<TestDomainEvent>)
            .IsAssignableFrom(typeof(IDomainEventHandler<TestDomainEvent>)).ShouldBeTrue();
    }

    [Fact]
    public async Task DomainEventHandler_ShouldExecuteCorrectly()
    {
        // Arrange
        var handler = new TestDomainEventHandler();
        var domainEvent = new TestDomainEvent { Message = "test message" };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.HandledEvents.Count.ShouldBe(1);
        handler.HandledEvents[0].Message.ShouldBe("test message");
    }

    [Fact]
    public async Task MultipleDomainEventHandlers_ShouldAllExecute()
    {
        // Arrange
        var handler1 = new TestDomainEventHandler();
        var handler2 = new AnotherTestDomainEventHandler();
        var domainEvent = new TestDomainEvent { Message = "test message" };

        // Act
        await handler1.Handle(domainEvent, CancellationToken.None);
        await handler2.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler1.HandledEvents.Count.ShouldBe(1);
        handler2.HandledEvents.Count.ShouldBe(1);
    }

    private class TestDomainEvent : IDomainEvent
    {
        public string Message { get; set; } = string.Empty;
    }

    private class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public List<TestDomainEvent> HandledEvents { get; } = new();

        public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            HandledEvents.Add(notification);
            return Task.CompletedTask;
        }
    }

    private class AnotherTestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public List<TestDomainEvent> HandledEvents { get; } = new();

        public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            HandledEvents.Add(notification);
            return Task.CompletedTask;
        }
    }
}
