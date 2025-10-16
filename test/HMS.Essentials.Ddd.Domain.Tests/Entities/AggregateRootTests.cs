using HMS.Essentials.Domain.Entities;
using HMS.Essentials.MediatR;
using Shouldly;

namespace HMS.Essentials.Ddd.Domain.Tests.Entities;

public class AggregateRootTests
{
    [Fact]
    public void AggregateRoot_ShouldHaveEmptyDomainEventsOnCreation()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot();

        // Assert
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToCollection()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var domainEvent = new TestDomainEvent();

        // Act
        aggregate.AddTestDomainEvent(domainEvent);

        // Assert
        aggregate.DomainEvents.Count.ShouldBe(1);
        aggregate.DomainEvents.ShouldContain(domainEvent);
    }

    [Fact]
    public void AddDomainEvent_ShouldAddMultipleEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Act
        aggregate.AddTestDomainEvent(event1);
        aggregate.AddTestDomainEvent(event2);

        // Assert
        aggregate.DomainEvents.Count.ShouldBe(2);
        aggregate.DomainEvents.ShouldContain(event1);
        aggregate.DomainEvents.ShouldContain(event2);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveEventFromCollection()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();
        aggregate.AddTestDomainEvent(event1);
        aggregate.AddTestDomainEvent(event2);

        // Act
        aggregate.RemoveTestDomainEvent(event1);

        // Assert
        aggregate.DomainEvents.Count.ShouldBe(1);
        aggregate.DomainEvents.ShouldNotContain(event1);
        aggregate.DomainEvents.ShouldContain(event2);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        aggregate.AddTestDomainEvent(new TestDomainEvent());
        aggregate.AddTestDomainEvent(new TestDomainEvent());
        aggregate.AddTestDomainEvent(new TestDomainEvent());

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void AggregateRoot_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot(123);

        // Assert
        aggregate.ShouldBeAssignableTo<Entity<int>>();
        aggregate.Id.ShouldBe(123);
    }

    [Fact]
    public void AggregateRoot_ShouldImplementIAggregateRoot()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot();

        // Assert
        aggregate.ShouldBeAssignableTo<IAggregateRoot<int>>();
        aggregate.ShouldBeAssignableTo<IAggregateRoot>();
    }

    [Fact]
    public void DomainEvents_ShouldBeReadOnly()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();

        // Act & Assert
        aggregate.DomainEvents.ShouldBeAssignableTo<IReadOnlyCollection<IDomainEvent>>();
    }

    [Fact]
    public void GuidAggregateRoot_ShouldGenerateGuidOnCreation()
    {
        // Arrange & Act
        var aggregate = new TestGuidAggregateRoot();

        // Assert
        aggregate.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void GuidAggregateRoot_ShouldGenerateUniqueGuids()
    {
        // Arrange & Act
        var aggregate1 = new TestGuidAggregateRoot();
        var aggregate2 = new TestGuidAggregateRoot();

        // Assert
        aggregate1.Id.ShouldNotBe(aggregate2.Id);
    }

    [Fact]
    public void AggregateRoot_WithIntKey_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var aggregate = new TestIntAggregateRoot(42);

        // Assert
        aggregate.ShouldBeAssignableTo<AggregateRoot>();
        aggregate.Id.ShouldBe(42);
    }

    // Test classes
    private class TestAggregateRoot : AggregateRoot<int>
    {
        public TestAggregateRoot() : base()
        {
        }
        
        public TestAggregateRoot(int id) : base(id)
        {
        }
        
        public void AddTestDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }

        public void RemoveTestDomainEvent(IDomainEvent domainEvent)
        {
            RemoveDomainEvent(domainEvent);
        }
    }

    private class TestGuidAggregateRoot : GuidAggregateRoot
    {
    }

    private class TestIntAggregateRoot : AggregateRoot
    {
        public TestIntAggregateRoot() : base()
        {
        }
        
        public TestIntAggregateRoot(int id) : base(id)
        {
        }
    }

    private class TestDomainEvent : IDomainEvent
    {
    }
}
