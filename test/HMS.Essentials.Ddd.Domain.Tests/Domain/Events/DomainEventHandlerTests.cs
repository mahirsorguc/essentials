using MediatR;

namespace HMS.Essentials.Domain.Events;

public class DomainEventHandlerTests
{
    [Fact]
    public void DomainEventHandler_ShouldBeAbstract()
    {
        // Assert
        typeof(DomainEventHandler<>).IsAbstract.ShouldBeTrue();
    }

    [Fact]
    public void DomainEventHandler_ShouldImplementIDomainEventHandler()
    {
        // Arrange & Act
        var handler = new TestDomainEventHandler();

        // Assert
        handler.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
        handler.ShouldBeAssignableTo<INotificationHandler<TestDomainEvent>>();
    }

    [Fact]
    public async Task DomainEventHandler_Handle_ShouldBeCallable()
    {
        // Arrange
        var handler = new TestDomainEventHandler();
        var domainEvent = new TestDomainEvent { Message = "Test" };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.HandledEvents.Count.ShouldBe(1);
        handler.HandledEvents[0].Message.ShouldBe("Test");
    }

    [Fact]
    public async Task DomainEventHandler_ShouldHandleMultipleEvents()
    {
        // Arrange
        var handler = new TestDomainEventHandler();
        var event1 = new TestDomainEvent { Message = "First" };
        var event2 = new TestDomainEvent { Message = "Second" };
        var event3 = new TestDomainEvent { Message = "Third" };

        // Act
        await handler.Handle(event1, CancellationToken.None);
        await handler.Handle(event2, CancellationToken.None);
        await handler.Handle(event3, CancellationToken.None);

        // Assert
        handler.HandledEvents.Count.ShouldBe(3);
        handler.HandledEvents[0].Message.ShouldBe("First");
        handler.HandledEvents[1].Message.ShouldBe("Second");
        handler.HandledEvents[2].Message.ShouldBe("Third");
    }

    [Fact]
    public async Task DomainEventHandler_ShouldRespectCancellationToken()
    {
        // Arrange
        var handler = new CancellableEventHandler();
        var domainEvent = new TestDomainEvent { Message = "Test" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await handler.Handle(domainEvent, cts.Token);
        });
    }

    [Fact]
    public async Task DomainEventHandler_ShouldSupportAsyncOperations()
    {
        // Arrange
        var handler = new AsyncEventHandler();
        var domainEvent = new TestDomainEvent { Message = "Async Test" };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.WasExecuted.ShouldBeTrue();
        handler.ExecutedMessage.ShouldBe("Async Test");
    }

    [Fact]
    public void DomainEventHandler_ShouldSupportInheritance()
    {
        // Arrange & Act
        var handler = new SpecializedEventHandler();

        // Assert
        handler.ShouldBeAssignableTo<DomainEventHandler<TestDomainEvent>>();
        handler.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
    }

    [Fact]
    public async Task DomainEventHandler_CanAccessEventProperties()
    {
        // Arrange
        var handler = new PropertyAccessingHandler();
        var domainEvent = new ComplexDomainEvent
        {
            Id = Guid.NewGuid(),
            Value = 42,
            Name = "Test Event"
        };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.ProcessedId.ShouldBe(domainEvent.Id);
        handler.ProcessedValue.ShouldBe(42);
        handler.ProcessedName.ShouldBe("Test Event");
    }

    [Fact]
    public async Task MultipleDomainEventHandlers_CanHandleSameEvent()
    {
        // Arrange
        var handler1 = new TestDomainEventHandler();
        var handler2 = new TestDomainEventHandler();
        var domainEvent = new TestDomainEvent { Message = "Shared Event" };

        // Act
        await handler1.Handle(domainEvent, CancellationToken.None);
        await handler2.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler1.HandledEvents.Count.ShouldBe(1);
        handler2.HandledEvents.Count.ShouldBe(1);
        handler1.HandledEvents[0].Message.ShouldBe("Shared Event");
        handler2.HandledEvents[0].Message.ShouldBe("Shared Event");
    }

    [Fact]
    public async Task DomainEventHandler_ShouldHandleNullProperties()
    {
        // Arrange
        var handler = new NullablePropertyHandler();
        var domainEvent = new EventWithNullables
        {
            RequiredValue = "Required",
            OptionalValue = null
        };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.HandledEvent.ShouldNotBeNull();
        handler.HandledEvent.RequiredValue.ShouldBe("Required");
        handler.HandledEvent.OptionalValue.ShouldBeNull();
    }

    [Fact]
    public async Task DomainEventHandler_CanPerformSideEffects()
    {
        // Arrange
        var handler = new SideEffectHandler();
        var domainEvent = new TestDomainEvent { Message = "Side Effect" };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.SideEffectExecuted.ShouldBeTrue();
        handler.ExecutionCount.ShouldBe(1);
    }

    // Test implementations
    private class TestDomainEvent : DomainEvent
    {
        public string Message { get; set; } = string.Empty;
    }

    private class ComplexDomainEvent : DomainEvent
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class EventWithNullables : DomainEvent
    {
        public string RequiredValue { get; set; } = string.Empty;
        public string? OptionalValue { get; set; }
    }

    private class TestDomainEventHandler : DomainEventHandler<TestDomainEvent>
    {
        public List<TestDomainEvent> HandledEvents { get; } = new();

        public override Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            HandledEvents.Add(notification);
            return Task.CompletedTask;
        }
    }

    private class CancellableEventHandler : DomainEventHandler<TestDomainEvent>
    {
        public override Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
    }

    private class AsyncEventHandler : DomainEventHandler<TestDomainEvent>
    {
        public bool WasExecuted { get; private set; }
        public string ExecutedMessage { get; private set; } = string.Empty;

        public override async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            WasExecuted = true;
            ExecutedMessage = notification.Message;
        }
    }

    private class SpecializedEventHandler : DomainEventHandler<TestDomainEvent>
    {
        public override Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private class PropertyAccessingHandler : DomainEventHandler<ComplexDomainEvent>
    {
        public Guid ProcessedId { get; private set; }
        public int ProcessedValue { get; private set; }
        public string ProcessedName { get; private set; } = string.Empty;

        public override Task Handle(ComplexDomainEvent notification, CancellationToken cancellationToken)
        {
            ProcessedId = notification.Id;
            ProcessedValue = notification.Value;
            ProcessedName = notification.Name;
            return Task.CompletedTask;
        }
    }

    private class NullablePropertyHandler : DomainEventHandler<EventWithNullables>
    {
        public EventWithNullables? HandledEvent { get; private set; }

        public override Task Handle(EventWithNullables notification, CancellationToken cancellationToken)
        {
            HandledEvent = notification;
            return Task.CompletedTask;
        }
    }

    private class SideEffectHandler : DomainEventHandler<TestDomainEvent>
    {
        public bool SideEffectExecuted { get; private set; }
        public int ExecutionCount { get; private set; }

        public override Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            SideEffectExecuted = true;
            ExecutionCount++;
            return Task.CompletedTask;
        }
    }
}
