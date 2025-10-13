using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.DomainEvents.Tests;

public class IDomainEventTests
{
    [Fact]
    public void IDomainEvent_ShouldImplementINotification()
    {
        // Assert
        typeof(INotification).IsAssignableFrom(typeof(IDomainEvent)).ShouldBeTrue();
    }

    [Fact]
    public void ConcreteDomainEvent_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        domainEvent.ShouldBeAssignableTo<INotification>();
    }

    [Fact]
    public void IDomainEvent_ShouldBeMarkerInterface()
    {
        // Assert - IDomainEvent should have no methods or properties
        var methods = typeof(IDomainEvent).GetMethods();
        var properties = typeof(IDomainEvent).GetProperties();

        // Only inherited methods from INotification should exist
        methods.Length.ShouldBe(0);
        properties.Length.ShouldBe(0);
    }

    [Fact]
    public void MultipleDomainEvents_ShouldBeDistinct()
    {
        // Arrange & Act
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Assert
        event1.ShouldNotBeSameAs(event2);
    }

    [Fact]
    public void DomainEvent_WithProperties_ShouldStoreData()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventWithProperties
        {
            EntityId = 123,
            Action = "Created",
            UserId = "user-001"
        };

        // Assert
        domainEvent.EntityId.ShouldBe(123);
        domainEvent.Action.ShouldBe("Created");
        domainEvent.UserId.ShouldBe("user-001");
    }

    [Fact]
    public void DomainEvent_ShouldRepresentSomethingThatHappened()
    {
        // This test demonstrates that domain events represent
        // past occurrences in the domain
        
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        // Domain events should be immutable and represent past facts
    }

    [Fact]
    public void DifferentDomainEvents_ShouldBeDistinct()
    {
        // Arrange & Act
        var userCreatedEvent = new UserCreatedEvent { UserId = 1 };
        var orderPlacedEvent = new OrderPlacedEvent { OrderId = 100 };

        // Assert
        userCreatedEvent.ShouldBeAssignableTo<IDomainEvent>();
        orderPlacedEvent.ShouldBeAssignableTo<IDomainEvent>();
        ((object)userCreatedEvent).ShouldNotBe((object)orderPlacedEvent);
    }

    [Fact]
    public void DomainEvent_ShouldSupportComplexData()
    {
        // Arrange & Act
        var domainEvent = new ComplexDomainEvent
        {
            AggregateId = Guid.NewGuid(),
            Changes = new Dictionary<string, object>
            {
                { "Name", "John Doe" },
                { "Age", 30 }
            },
            Tags = new List<string> { "important", "audit" }
        };

        // Assert
        domainEvent.Changes.Count.ShouldBe(2);
        domainEvent.Tags.Count.ShouldBe(2);
    }

    private class TestDomainEvent : IDomainEvent
    {
    }

    private class TestDomainEventWithProperties : IDomainEvent
    {
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    private class UserCreatedEvent : IDomainEvent
    {
        public int UserId { get; set; }
    }

    private class OrderPlacedEvent : IDomainEvent
    {
        public int OrderId { get; set; }
    }

    private class ComplexDomainEvent : IDomainEvent
    {
        public Guid AggregateId { get; set; }
        public Dictionary<string, object> Changes { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}
