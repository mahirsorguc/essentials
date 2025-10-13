using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.DomainEvents.Tests;

public class DomainEventBaseTests
{
    [Fact]
    public void DomainEventBase_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventBase();

        // Assert
        domainEvent.ShouldBeAssignableTo<DomainEventBase>();
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        domainEvent.ShouldBeAssignableTo<INotification>();
    }

    [Fact]
    public void DomainEventBase_ShouldSetOccurredOnOnCreation()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var domainEvent = new TestDomainEventBase();

        // Assert
        var afterCreation = DateTimeOffset.UtcNow;
        domainEvent.OccurredOn.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        domainEvent.OccurredOn.ShouldBeLessThanOrEqualTo(afterCreation);
    }

    [Fact]
    public void DomainEventBase_ShouldGenerateUniqueEventId()
    {
        // Arrange & Act
        var event1 = new TestDomainEventBase();
        var event2 = new TestDomainEventBase();

        // Assert
        event1.EventId.ShouldNotBe(Guid.Empty);
        event2.EventId.ShouldNotBe(Guid.Empty);
        event1.EventId.ShouldNotBe(event2.EventId);
    }

    [Fact]
    public void DomainEventBase_OccurredOn_ShouldBeReadOnly()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventBase();
        var initialOccurredOn = domainEvent.OccurredOn;

        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Assert - OccurredOn should not change
        domainEvent.OccurredOn.ShouldBe(initialOccurredOn);
    }

    [Fact]
    public void DomainEventBase_EventId_ShouldBeReadOnly()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventBase();
        var initialEventId = domainEvent.EventId;

        // Assert - EventId should not change
        domainEvent.EventId.ShouldBe(initialEventId);
    }

    [Fact]
    public void DomainEventBase_WithProperties_ShouldWorkAsRecord()
    {
        // Arrange & Act
        var event1 = new TestDomainEventWithProperties("test", 123);
        var event2 = new TestDomainEventWithProperties("test", 123);

        // Assert - Records with same values should be equal (except OccurredOn and EventId)
        event1.Action.ShouldBe(event2.Action);
        event1.EntityId.ShouldBe(event2.EntityId);
    }

    [Fact]
    public void DomainEventBase_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange & Act
        var event1 = new TestDomainEventWithProperties("Created", 1);
        var event2 = new TestDomainEventWithProperties("Updated", 2);

        // Assert
        event1.ShouldNotBe(event2);
        event1.Action.ShouldNotBe(event2.Action);
        event1.EntityId.ShouldNotBe(event2.EntityId);
    }

    [Fact]
    public void MultipleDomainEvents_ShouldHaveDifferentEventIds()
    {
        // Arrange & Act
        var events = Enumerable.Range(1, 100)
            .Select(_ => new TestDomainEventBase())
            .ToList();

        // Assert
        var uniqueEventIds = events.Select(e => e.EventId).Distinct().Count();
        uniqueEventIds.ShouldBe(100);
    }

    [Fact]
    public void MultipleDomainEvents_ShouldHaveDifferentOccurrenceTimes()
    {
        // Arrange & Act
        var event1 = new TestDomainEventBase();
        Thread.Sleep(10);
        var event2 = new TestDomainEventBase();

        // Assert
        event2.OccurredOn.ShouldBeGreaterThan(event1.OccurredOn);
    }

    [Fact]
    public void DomainEventBase_ShouldBeRecord()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventWithProperties("Created", 123);
        var modifiedEvent = domainEvent with { Action = "Updated" };

        // Assert
        modifiedEvent.Action.ShouldBe("Updated");
        modifiedEvent.EntityId.ShouldBe(123);
        domainEvent.Action.ShouldBe("Created"); // Original should be unchanged
        
        // EventId and OccurredOn should be copied
        modifiedEvent.EventId.ShouldBe(domainEvent.EventId);
        modifiedEvent.OccurredOn.ShouldBe(domainEvent.OccurredOn);
    }

    [Fact]
    public void DomainEventBase_WithComplexProperties_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var domainEvent = new ComplexDomainEvent(
            Guid.NewGuid(),
            new Dictionary<string, object> { { "key1", "value1" } },
            new List<string> { "tag1", "tag2" }
        );

        // Assert
        domainEvent.OccurredOn.ShouldNotBe(default(DateTimeOffset));
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
        domainEvent.Metadata.Count.ShouldBe(1);
        domainEvent.Tags.Count.ShouldBe(2);
    }

    [Fact]
    public void DomainEventBase_ShouldSupportInheritance()
    {
        // Arrange & Act
        var userEvent = new UserCreatedDomainEvent(123, "john.doe@example.com");
        var orderEvent = new OrderPlacedDomainEvent(456, 99.99m);

        // Assert
        userEvent.ShouldBeAssignableTo<DomainEventBase>();
        orderEvent.ShouldBeAssignableTo<DomainEventBase>();
        userEvent.UserId.ShouldBe(123);
        orderEvent.OrderId.ShouldBe(456);
    }

    [Fact]
    public void DomainEventBase_EventId_ShouldBeValidGuid()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventBase();

        // Assert
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
        Guid.TryParse(domainEvent.EventId.ToString(), out var parsedGuid).ShouldBeTrue();
        parsedGuid.ShouldBe(domainEvent.EventId);
    }

    [Fact]
    public void DomainEventBase_OccurredOn_ShouldBeUtcTime()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEventBase();

        // Assert
        // OccurredOn should be in UTC
        domainEvent.OccurredOn.Offset.ShouldBe(TimeSpan.Zero);
    }

    private record TestDomainEventBase : DomainEventBase
    {
    }

    private record TestDomainEventWithProperties(string Action, int EntityId) : DomainEventBase
    {
    }

    private record ComplexDomainEvent(
        Guid AggregateId,
        Dictionary<string, object> Metadata,
        List<string> Tags
    ) : DomainEventBase
    {
    }

    private record UserCreatedDomainEvent(int UserId, string Email) : DomainEventBase
    {
    }

    private record OrderPlacedDomainEvent(int OrderId, decimal Amount) : DomainEventBase
    {
    }
}
