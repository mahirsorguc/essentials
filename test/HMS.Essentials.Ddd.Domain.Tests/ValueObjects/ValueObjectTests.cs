using HMS.Essentials.Domain.ValueObjects;
using Shouldly;

namespace HMS.Essentials.Ddd.Domain.Tests.ValueObjects;

public class ValueObjectTests
{
    [Fact]
    public void ValueObjects_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = new TestAddress("123 Main St", "Springfield");
        var address2 = new TestAddress("123 Main St", "Springfield");

        // Act & Assert
        address1.ShouldBe(address2);
        address1.Equals(address2).ShouldBeTrue();
        (address1 == address2).ShouldBeTrue();
    }

    [Fact]
    public void ValueObjects_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = new TestAddress("123 Main St", "Springfield");
        var address2 = new TestAddress("456 Oak Ave", "Springfield");

        // Act & Assert
        address1.ShouldNotBe(address2);
        address1.Equals(address2).ShouldBeFalse();
        (address1 != address2).ShouldBeTrue();
    }

    [Fact]
    public void ValueObjects_WithSameValues_ShouldHaveSameHashCode()
    {
        // Arrange
        var address1 = new TestAddress("123 Main St", "Springfield");
        var address2 = new TestAddress("123 Main St", "Springfield");

        // Act & Assert
        address1.GetHashCode().ShouldBe(address2.GetHashCode());
    }

    [Fact]
    public void ValueObject_ComparedToNull_ShouldNotBeEqual()
    {
        // Arrange
        var address = new TestAddress("123 Main St", "Springfield");

        // Act & Assert
        address.Equals(null).ShouldBeFalse();
        (address == null).ShouldBeFalse();
        (address != null).ShouldBeTrue();
    }

    [Fact]
    public void ValueObject_ComparedToDifferentType_ShouldNotBeEqual()
    {
        // Arrange
        var address = new TestAddress("123 Main St", "Springfield");
        var money = new TestMoney(100);

        // Act & Assert
        address.Equals(money).ShouldBeFalse();
    }

    [Fact]
    public void GetCopy_ShouldCreateEqualCopy()
    {
        // Arrange
        var original = new TestAddress("123 Main St", "Springfield");

        // Act
        var copy = (TestAddress)original.GetCopy();

        // Assert
        copy.ShouldBe(original);
        copy.ShouldNotBeSameAs(original);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var address = new TestAddress("123 Main St", "Springfield");

        // Act
        var result = address.ToString();

        // Assert
        result.ShouldContain("TestAddress");
        result.ShouldContain("123 Main St");
        result.ShouldContain("Springfield");
    }

    [Fact]
    public void NullValueObjects_ShouldBeEqual()
    {
        // Arrange
        TestAddress? address1 = null;
        TestAddress? address2 = null;

        // Act & Assert
        (address1 == address2).ShouldBeTrue();
        (address1 != address2).ShouldBeFalse();
    }

    [Fact]
    public void ValueObject_WithNullComponent_ShouldHandleCorrectly()
    {
        // Arrange
        var vo1 = new TestValueObjectWithNullable("test", null);
        var vo2 = new TestValueObjectWithNullable("test", null);

        // Act & Assert
        vo1.ShouldBe(vo2);
    }

    [Fact]
    public void ValueObject_WithComplexComponents_ShouldWorkCorrectly()
    {
        // Arrange
        var vo1 = new TestComplexValueObject(123, "test", true);
        var vo2 = new TestComplexValueObject(123, "test", true);
        var vo3 = new TestComplexValueObject(123, "test", false);

        // Act & Assert
        vo1.ShouldBe(vo2);
        vo1.ShouldNotBe(vo3);
    }

    // Test classes
    private class TestAddress : ValueObject
    {
        public string Street { get; }
        public string City { get; }

        public TestAddress(string street, string city)
        {
            Street = street;
            City = city;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
        }
    }

    private class TestMoney : ValueObject
    {
        public decimal Amount { get; }

        public TestMoney(decimal amount)
        {
            Amount = amount;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
        }
    }

    private class TestValueObjectWithNullable : ValueObject
    {
        public string Name { get; }
        public string? OptionalValue { get; }

        public TestValueObjectWithNullable(string name, string? optionalValue)
        {
            Name = name;
            OptionalValue = optionalValue;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return OptionalValue;
        }
    }

    private class TestComplexValueObject : ValueObject
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsActive { get; }

        public TestComplexValueObject(int id, string name, bool isActive)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Id;
            yield return Name;
            yield return IsActive;
        }
    }
}
