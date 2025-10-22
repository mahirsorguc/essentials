using MediatR;

namespace HMS.Essentials.Domain.Events;

public class IDomainEventHandlerInterfaceTests
{
    [Fact]
    public void IDomainEventHandler_ShouldBeAnInterface()
    {
        // Assert
        typeof(IDomainEventHandler<>).IsInterface.ShouldBeTrue();
    }

    [Fact]
    public void IDomainEventHandler_ShouldExtendINotificationHandler()
    {
        // Assert
        var handlerType = typeof(IDomainEventHandler<TestDomainEvent>);
        var notificationHandlerType = typeof(INotificationHandler<TestDomainEvent>);
        
        notificationHandlerType.IsAssignableFrom(handlerType).ShouldBeTrue();
    }

    [Fact]
    public void IDomainEventHandler_ShouldHaveGenericTypeConstraint()
    {
        // Arrange & Act
        var genericType = typeof(IDomainEventHandler<>);
        var constraints = genericType.GetGenericArguments()[0].GetGenericParameterConstraints();

        // Assert
        constraints.ShouldContain(c => c == typeof(DomainEvent));
    }

    [Fact]
    public void IDomainEventHandler_ShouldBeImplementableByConcreteClasses()
    {
        // Arrange & Act
        var handler = new TestDomainEventHandlerImpl();

        // Assert
        handler.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
        handler.ShouldBeAssignableTo<INotificationHandler<TestDomainEvent>>();
    }

    [Fact]
    public async Task IDomainEventHandler_Handle_ShouldBeCallable()
    {
        // Arrange
        var handler = new TestDomainEventHandlerImpl();
        var domainEvent = new TestDomainEvent { Value = 42 };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.HandledEvents.Count.ShouldBe(1);
        handler.HandledEvents[0].Value.ShouldBe(42);
    }

    [Fact]
    public void MultipleDomainEventHandlers_CanImplementSameInterface()
    {
        // Arrange & Act
        var handler1 = new TestDomainEventHandlerImpl();
        var handler2 = new AnotherDomainEventHandlerImpl();

        // Assert
        handler1.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
        handler2.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
    }

    [Fact]
    public void IDomainEventHandler_CanBeUsedPolymorphically()
    {
        // Arrange & Act
        IDomainEventHandler<TestDomainEvent> handler1 = new TestDomainEventHandlerImpl();
        IDomainEventHandler<TestDomainEvent> handler2 = new AnotherDomainEventHandlerImpl();
        var handlers = new List<IDomainEventHandler<TestDomainEvent>> { handler1, handler2 };

        // Assert
        handlers.Count.ShouldBe(2);
        handlers[0].ShouldBeOfType<TestDomainEventHandlerImpl>();
        handlers[1].ShouldBeOfType<AnotherDomainEventHandlerImpl>();
    }

    [Fact]
    public void IDomainEventHandler_ShouldAllowDifferentEventTypes()
    {
        // Arrange & Act
        var handler1 = new TestDomainEventHandlerImpl();
        var handler2 = new AnotherEventTypeHandler();

        // Assert
        handler1.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
        handler2.ShouldBeAssignableTo<IDomainEventHandler<AnotherDomainEvent>>();
    }

    [Fact]
    public async Task IDomainEventHandler_ShouldSupportAsyncHandling()
    {
        // Arrange
        var handler = new AsyncDomainEventHandler();
        var domainEvent = new TestDomainEvent { Value = 100 };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.WasExecuted.ShouldBeTrue();
        handler.ExecutedValue.ShouldBe(100);
    }

    [Fact]
    public void IDomainEventHandler_AsINotificationHandler_ShouldBeCompatible()
    {
        // Arrange & Act
        var handler = new TestDomainEventHandlerImpl();
        INotificationHandler<TestDomainEvent> notificationHandler = handler;

        // Assert
        notificationHandler.ShouldNotBeNull();
        notificationHandler.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEvent>>();
    }

    [Fact]
    public void IDomainEventHandler_ShouldNotHaveOwnMethods()
    {
        // IDomainEventHandler inherits everything from INotificationHandler
        // and adds the constraint that TDomainEvent must be DomainEvent
        
        // Assert
        var methods = typeof(IDomainEventHandler<>).GetMethods();
        // Should only have methods inherited from INotificationHandler
        methods.Length.ShouldBe(0);
    }

    [Fact]
    public async Task IDomainEventHandler_CanHandleDerivedEvents()
    {
        // Arrange
        var handler = new SpecializedEventHandler();
        var domainEvent = new SpecializedDomainEvent
        {
            BaseValue = "Base",
            SpecialValue = "Special"
        };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.HandledEvent.ShouldNotBeNull();
        handler.HandledEvent.BaseValue.ShouldBe("Base");
        handler.HandledEvent.SpecialValue.ShouldBe("Special");
    }

    // Test implementations
    private class TestDomainEvent : DomainEvent
    {
        public int Value { get; set; }
    }

    private class AnotherDomainEvent : DomainEvent
    {
        public string Name { get; set; } = string.Empty;
    }

    private class SpecializedDomainEvent : DomainEvent
    {
        public string BaseValue { get; set; } = string.Empty;
        public string SpecialValue { get; set; } = string.Empty;
    }

    private class TestDomainEventHandlerImpl : IDomainEventHandler<TestDomainEvent>
    {
        public List<TestDomainEvent> HandledEvents { get; } = new();

        public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            HandledEvents.Add(notification);
            return Task.CompletedTask;
        }
    }

    private class AnotherDomainEventHandlerImpl : IDomainEventHandler<TestDomainEvent>
    {
        public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private class AnotherEventTypeHandler : IDomainEventHandler<AnotherDomainEvent>
    {
        public Task Handle(AnotherDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private class AsyncDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public bool WasExecuted { get; private set; }
        public int ExecutedValue { get; private set; }

        public async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            WasExecuted = true;
            ExecutedValue = notification.Value;
        }
    }

    private class SpecializedEventHandler : IDomainEventHandler<SpecializedDomainEvent>
    {
        public SpecializedDomainEvent? HandledEvent { get; private set; }

        public Task Handle(SpecializedDomainEvent notification, CancellationToken cancellationToken)
        {
            HandledEvent = notification;
            return Task.CompletedTask;
        }
    }
}
