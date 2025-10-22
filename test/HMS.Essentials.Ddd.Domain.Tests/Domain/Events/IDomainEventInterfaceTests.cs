using MediatR;

namespace HMS.Essentials.Domain.Events;

public class IDomainEventInterfaceTests
{
    [Fact]
    public void IDomainEvent_ShouldExtendINotification()
    {
        // Assert
        typeof(INotification).IsAssignableFrom(typeof(IDomainEvent)).ShouldBeTrue();
    }

    [Fact]
    public void IDomainEvent_ShouldBeAnInterface()
    {
        // Assert
        typeof(IDomainEvent).IsInterface.ShouldBeTrue();
    }

    [Fact]
    public void IDomainEvent_ShouldBeMarkerInterface()
    {
        // Assert - IDomainEvent should have no methods or properties beyond INotification
        var methods = typeof(IDomainEvent).GetMethods();
        var properties = typeof(IDomainEvent).GetProperties();

        // Only inherited methods and properties should exist
        methods.Length.ShouldBe(0);
        properties.Length.ShouldBe(0);
    }

    [Fact]
    public void ConcreteDomainEvent_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventImpl();

        // Assert
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        domainEvent.ShouldBeAssignableTo<INotification>();
    }

    [Fact]
    public void MultipleDomainEventImplementations_ShouldBeDistinct()
    {
        // Arrange & Act
        var event1 = new TestDomainEventImpl();
        var event2 = new AnotherDomainEventImpl();

        // Assert
        event1.ShouldBeAssignableTo<IDomainEvent>();
        event2.ShouldBeAssignableTo<IDomainEvent>();
        event1.ShouldNotBeSameAs(event2);
    }

    [Fact]
    public void IDomainEvent_CanBeImplementedByClass()
    {
        // Arrange & Act
        var domainEvent = new ClassBasedDomainEvent { Value = 100 };

        // Assert
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        domainEvent.Value.ShouldBe(100);
    }

    [Fact]
    public void IDomainEvent_CanBeImplementedByRecord()
    {
        // Arrange & Act
        var domainEvent = new RecordBasedDomainEvent(42, "Test");

        // Assert
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        domainEvent.Id.ShouldBe(42);
        domainEvent.Name.ShouldBe("Test");
    }

    [Fact]
    public void IDomainEvent_ShouldSupportCustomProperties()
    {
        // Arrange & Act
        var domainEvent = new CustomPropertiesDomainEvent
        {
            EntityId = Guid.NewGuid(),
            Action = "Created",
            Timestamp = DateTimeOffset.UtcNow
        };

        // Assert
        domainEvent.EntityId.ShouldNotBe(Guid.Empty);
        domainEvent.Action.ShouldBe("Created");
        domainEvent.Timestamp.ShouldNotBe(default(DateTimeOffset));
    }

    [Fact]
    public void IDomainEvent_CanBeUsedAsNotification()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventImpl();
        INotification notification = domainEvent;

        // Assert
        notification.ShouldNotBeNull();
        notification.ShouldBeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void IDomainEvent_ShouldAllowPolymorphicUsage()
    {
        // Arrange & Act
        IDomainEvent domainEvent1 = new TestDomainEventImpl();
        IDomainEvent domainEvent2 = new AnotherDomainEventImpl();
        var events = new List<IDomainEvent> { domainEvent1, domainEvent2 };

        // Assert
        events.Count.ShouldBe(2);
        events[0].ShouldBeOfType<TestDomainEventImpl>();
        events[1].ShouldBeOfType<AnotherDomainEventImpl>();
    }

    [Fact]
    public void DomainEvent_FromDifferentNamespaces_ShouldStillImplementIDomainEvent()
    {
        // This demonstrates that IDomainEvent from HMS.Essentials.Domain.Events
        // can be implemented by various domain event types
        
        // Arrange & Act
        var event1 = new TestDomainEventImpl();
        var event2 = new ClassBasedDomainEvent();

        // Assert
        event1.ShouldBeAssignableTo<IDomainEvent>();
        event2.ShouldBeAssignableTo<IDomainEvent>();
    }

    // Test implementations
    private class TestDomainEventImpl : IDomainEvent
    {
    }

    private class AnotherDomainEventImpl : IDomainEvent
    {
    }

    private class ClassBasedDomainEvent : IDomainEvent
    {
        public int Value { get; set; }
    }

    private record RecordBasedDomainEvent(int Id, string Name) : IDomainEvent;

    private class CustomPropertiesDomainEvent : IDomainEvent
    {
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
    }
}
