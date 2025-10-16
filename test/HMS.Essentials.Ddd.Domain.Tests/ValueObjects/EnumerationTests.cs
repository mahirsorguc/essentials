using HMS.Essentials.Domain.ValueObjects;
using Shouldly;

namespace HMS.Essentials.Ddd.Domain.Tests.ValueObjects;

public class EnumerationTests
{
    [Fact]
    public void Enumeration_ShouldHaveIdAndName()
    {
        // Arrange & Act
        var status = TestStatus.Active;

        // Assert
        status.Id.ShouldBe(1);
        status.Name.ShouldBe(nameof(TestStatus.Active));
    }

    [Fact]
    public void GetAll_ShouldReturnAllEnumerationValues()
    {
        // Arrange & Act
        var allStatuses = Enumeration.GetAll<TestStatus>().ToList();

        // Assert
        allStatuses.Count.ShouldBe(3);
        allStatuses.ShouldContain(TestStatus.Active);
        allStatuses.ShouldContain(TestStatus.Inactive);
        allStatuses.ShouldContain(TestStatus.Pending);
    }

    [Fact]
    public void FromId_ShouldReturnCorrectEnumeration()
    {
        // Arrange & Act
        var status = Enumeration.FromId<TestStatus>(2);

        // Assert
        status.ShouldBe(TestStatus.Inactive);
    }

    [Fact]
    public void FromId_WithInvalidId_ShouldThrowException()
    {
        // Arrange, Act & Assert
        Should.Throw<InvalidOperationException>(() => 
            Enumeration.FromId<TestStatus>(999));
    }

    [Fact]
    public void FromDisplayName_ShouldReturnCorrectEnumeration()
    {
        // Arrange & Act
        var status = Enumeration.FromDisplayName<TestStatus>("Active");

        // Assert
        status.ShouldBe(TestStatus.Active);
    }

    [Fact]
    public void FromDisplayName_WithInvalidName_ShouldThrowException()
    {
        // Arrange, Act & Assert
        Should.Throw<InvalidOperationException>(() => 
            Enumeration.FromDisplayName<TestStatus>("Invalid"));
    }

    [Fact]
    public void TryFromId_WithValidId_ShouldReturnTrue()
    {
        // Arrange & Act
        var result = Enumeration.TryFromId<TestStatus>(1, out var status);

        // Assert
        result.ShouldBeTrue();
        status.ShouldBe(TestStatus.Active);
    }

    [Fact]
    public void TryFromId_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = Enumeration.TryFromId<TestStatus>(999, out var status);

        // Assert
        result.ShouldBeFalse();
        status.ShouldBeNull();
    }

    [Fact]
    public void TryFromDisplayName_WithValidName_ShouldReturnTrue()
    {
        // Arrange & Act
        var result = Enumeration.TryFromDisplayName<TestStatus>("Inactive", out var status);

        // Assert
        result.ShouldBeTrue();
        status.ShouldBe(TestStatus.Inactive);
    }

    [Fact]
    public void TryFromDisplayName_WithInvalidName_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = Enumeration.TryFromDisplayName<TestStatus>("Invalid", out var status);

        // Assert
        result.ShouldBeFalse();
        status.ShouldBeNull();
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var status1 = TestStatus.Active;
        var status2 = Enumeration.FromId<TestStatus>(1);

        // Act & Assert
        status1.Equals(status2).ShouldBeTrue();
        (status1 == status2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Inactive;

        // Act & Assert
        status1.Equals(status2).ShouldBeFalse();
        (status1 != status2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHash()
    {
        // Arrange
        var status1 = TestStatus.Active;
        var status2 = Enumeration.FromId<TestStatus>(1);

        // Act & Assert
        status1.GetHashCode().ShouldBe(status2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnName()
    {
        // Arrange
        var status = TestStatus.Active;

        // Act
        var result = status.ToString();

        // Assert
        result.ShouldBe("Active");
    }

    [Fact]
    public void CompareTo_ShouldCompareByIdValue()
    {
        // Arrange
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Inactive;
        var status3 = TestStatus.Pending;

        // Act & Assert
        status1.CompareTo(status2).ShouldBeLessThan(0);
        status2.CompareTo(status1).ShouldBeGreaterThan(0);
        status1.CompareTo(status1).ShouldBe(0);

        (status1 < status2).ShouldBeTrue();
        (status2 > status1).ShouldBeTrue();
        (status1 <= status2).ShouldBeTrue();
        (status3 >= status2).ShouldBeTrue();
    }

    [Fact]
    public void AbsoluteDifference_ShouldReturnCorrectValue()
    {
        // Arrange
        var status1 = TestStatus.Active;    // Id = 1
        var status2 = TestStatus.Pending;   // Id = 3

        // Act
        var difference = Enumeration.AbsoluteDifference(status1, status2);

        // Assert
        difference.ShouldBe(2);
    }

    [Fact]
    public void Enumeration_WithCustomBehavior_ShouldWork()
    {
        // Arrange
        var status = TestStatus.Active;

        // Act
        var canTransition = status.CanTransitionTo(TestStatus.Inactive);

        // Assert
        canTransition.ShouldBeTrue();
    }

    [Fact]
    public void CompareTo_WithNonEnumeration_ShouldThrowException()
    {
        // Arrange
        var status = TestStatus.Active;

        // Act & Assert
        Should.Throw<ArgumentException>(() => status.CompareTo("string"));
    }

    [Fact]
    public void EqualityOperators_WithNull_ShouldWorkCorrectly()
    {
        // Arrange
        TestStatus? status1 = null;
        TestStatus? status2 = null;
        var status3 = TestStatus.Active;

        // Act & Assert
        (status1 == status2).ShouldBeTrue();
        (status1 != status3).ShouldBeTrue();
        (status3 != status1).ShouldBeTrue();
    }

    // Test enumeration class
    private class TestStatus : Enumeration
    {
        public static readonly TestStatus Active = new TestStatus(1, nameof(Active));
        public static readonly TestStatus Inactive = new TestStatus(2, nameof(Inactive));
        public static readonly TestStatus Pending = new TestStatus(3, nameof(Pending));

        private TestStatus(int id, string name) : base(id, name)
        {
        }

        public bool CanTransitionTo(TestStatus newStatus)
        {
            // Business logic example
            if (this == Active && newStatus == Inactive)
                return true;
            if (this == Pending && (newStatus == Active || newStatus == Inactive))
                return true;
            
            return false;
        }
    }
}
