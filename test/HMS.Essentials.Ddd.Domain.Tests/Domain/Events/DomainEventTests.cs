using MediatR;

namespace HMS.Essentials.Domain.Events;

public class DomainEventTests
{
    [Fact]
    public void DomainEvent_ShouldBeAbstract()
    {
        // Assert
        typeof(DomainEvent).IsAbstract.ShouldBeTrue();
    }

    [Fact]
    public void DomainEvent_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.ShouldBeAssignableTo<DomainEvent>();
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        domainEvent.ShouldBeAssignableTo<INotification>();
    }

    [Fact]
    public void DomainEvent_ShouldSupportInheritance()
    {
        // Arrange & Act
        var userCreatedEvent = new UserCreatedDomainEvent
        {
            UserId = 123,
            Email = "user@example.com"
        };

        // Assert
        userCreatedEvent.ShouldBeAssignableTo<DomainEvent>();
        userCreatedEvent.UserId.ShouldBe(123);
        userCreatedEvent.Email.ShouldBe("user@example.com");
    }

    [Fact]
    public void DomainEvent_WithProperties_ShouldStoreData()
    {
        // Arrange & Act
        var domainEvent = new OrderPlacedDomainEvent
        {
            OrderId = 456,
            Amount = 99.99m,
            CustomerId = 789
        };

        // Assert
        domainEvent.OrderId.ShouldBe(456);
        domainEvent.Amount.ShouldBe(99.99m);
        domainEvent.CustomerId.ShouldBe(789);
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
    public void DomainEvent_ShouldSupportComplexProperties()
    {
        // Arrange & Act
        var domainEvent = new ComplexDomainEvent
        {
            AggregateId = Guid.NewGuid(),
            Metadata = new Dictionary<string, object>
            {
                { "Action", "Create" },
                { "Timestamp", DateTime.UtcNow }
            },
            Tags = new List<string> { "important", "audit" }
        };

        // Assert
        domainEvent.AggregateId.ShouldNotBe(Guid.Empty);
        domainEvent.Metadata.Count.ShouldBe(2);
        domainEvent.Tags.Count.ShouldBe(2);
        domainEvent.Tags.ShouldContain("important");
        domainEvent.Tags.ShouldContain("audit");
    }

    [Fact]
    public void DomainEvent_ShouldBeInstantiable()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.ShouldNotBeNull();
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void DifferentDomainEventTypes_ShouldBeDistinguishable()
    {
        // Arrange & Act
        var userEvent = new UserCreatedDomainEvent { UserId = 1 };
        var orderEvent = new OrderPlacedDomainEvent { OrderId = 100 };

        // Assert
        userEvent.ShouldBeAssignableTo<DomainEvent>();
        orderEvent.ShouldBeAssignableTo<DomainEvent>();
        ((object)userEvent).ShouldNotBe((object)orderEvent);
        userEvent.GetType().ShouldNotBe(orderEvent.GetType());
    }

    [Fact]
    public void DomainEvent_ShouldRepresentPastOccurrence()
    {
        // This test demonstrates that domain events represent
        // something that has already happened in the domain
        
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.ShouldBeAssignableTo<IDomainEvent>();
        // Domain events should be treated as immutable facts about past occurrences
    }

    [Fact]
    public void DomainEvent_ShouldWorkWithNullableProperties()
    {
        // Arrange & Act
        var domainEvent = new DomainEventWithNullables
        {
            Name = "Test",
            Description = null,
            Value = 100
        };

        // Assert
        domainEvent.Name.ShouldBe("Test");
        domainEvent.Description.ShouldBeNull();
        domainEvent.Value.ShouldBe(100);
    }

    [Fact]
    public void DomainEvent_ShouldSupportNestedObjects()
    {
        // Arrange
        var address = new Address
        {
            Street = "123 Main St",
            City = "New York",
            ZipCode = "10001"
        };

        // Act
        var domainEvent = new CustomerAddressChangedEvent
        {
            CustomerId = 1,
            NewAddress = address
        };

        // Assert
        domainEvent.CustomerId.ShouldBe(1);
        domainEvent.NewAddress.ShouldNotBeNull();
        domainEvent.NewAddress.Street.ShouldBe("123 Main St");
        domainEvent.NewAddress.City.ShouldBe("New York");
    }

    // Test classes
    private class TestDomainEvent : DomainEvent
    {
    }

    private class UserCreatedDomainEvent : DomainEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    private class OrderPlacedDomainEvent : DomainEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
    }

    private class ComplexDomainEvent : DomainEvent
    {
        public Guid AggregateId { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    private class DomainEventWithNullables : DomainEvent
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Value { get; set; }
    }

    private class CustomerAddressChangedEvent : DomainEvent
    {
        public int CustomerId { get; set; }
        public Address NewAddress { get; set; } = new();
    }

    private class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }
}
