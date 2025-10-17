using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

/// <summary>
/// Tests for UnitOfWorkAttribute.
/// </summary>
public class UnitOfWorkAttributeTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute();

        // Assert
        attribute.IsEnabled.ShouldBeTrue();
        attribute.AutoCommit.ShouldBeTrue();
        attribute.AutoRollback.ShouldBeTrue();
    }

    [Fact]
    public void Constructor_WithIsEnabled_ShouldSetValue()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute(isEnabled: false);

        // Assert
        attribute.IsEnabled.ShouldBeFalse();
        attribute.AutoCommit.ShouldBeTrue(); // Still default
        attribute.AutoRollback.ShouldBeTrue(); // Still default
    }

    [Fact]
    public void Constructor_WithIsEnabledTrue_ShouldSetValue()
    {
        // Arrange & Act
        var attribute = new UnitOfWorkAttribute(isEnabled: true);

        // Assert
        attribute.IsEnabled.ShouldBeTrue();
        attribute.AutoCommit.ShouldBeTrue();
        attribute.AutoRollback.ShouldBeTrue();
    }

    [Fact]
    public void IsEnabled_CanBeSet()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute();

        // Act
        attribute.IsEnabled = false;

        // Assert
        attribute.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void AutoCommit_CanBeSet()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute();

        // Act
        attribute.AutoCommit = false;

        // Assert
        attribute.AutoCommit.ShouldBeFalse();
    }

    [Fact]
    public void AutoRollback_CanBeSet()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute();

        // Act
        attribute.AutoRollback = false;

        // Assert
        attribute.AutoRollback.ShouldBeFalse();
    }

    [Fact]
    public void AllProperties_CanBeSetToFalse()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute();

        // Act
        attribute.IsEnabled = false;
        attribute.AutoCommit = false;
        attribute.AutoRollback = false;

        // Assert
        attribute.IsEnabled.ShouldBeFalse();
        attribute.AutoCommit.ShouldBeFalse();
        attribute.AutoRollback.ShouldBeFalse();
    }

    [Fact]
    public void AllProperties_CanBeSetToTrue()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute(isEnabled: false);

        // Act
        attribute.IsEnabled = true;
        attribute.AutoCommit = true;
        attribute.AutoRollback = true;

        // Assert
        attribute.IsEnabled.ShouldBeTrue();
        attribute.AutoCommit.ShouldBeTrue();
        attribute.AutoRollback.ShouldBeTrue();
    }

    [Fact]
    public void Attribute_ShouldHaveCorrectAttributeUsage()
    {
        // Arrange
        var attributeType = typeof(UnitOfWorkAttribute);

        // Act
        var usageAttribute = attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        usageAttribute.ShouldNotBeNull();
        usageAttribute.ValidOn.ShouldBe(AttributeTargets.Class);
        usageAttribute.AllowMultiple.ShouldBeFalse();
        usageAttribute.Inherited.ShouldBeTrue();
    }

    [Fact]
    public void Attribute_InheritsFromAttribute()
    {
        // Arrange
        var attribute = new UnitOfWorkAttribute();

        // Assert
        attribute.ShouldBeAssignableTo<Attribute>();
    }

    [Fact]
    public void Attribute_CanBeAppliedToClass()
    {
        // Arrange
        var testCommandType = typeof(TestCommand);

        // Act
        var attribute = testCommandType.GetCustomAttributes(typeof(UnitOfWorkAttribute), false)
            .Cast<UnitOfWorkAttribute>()
            .FirstOrDefault();

        // Assert
        attribute.ShouldNotBeNull();
        attribute.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Attribute_WithCustomIsEnabled_CanBeAppliedToClass()
    {
        // Arrange
        var testCommandType = typeof(TestCommandWithCustomValues);

        // Act
        var attribute = testCommandType.GetCustomAttributes(typeof(UnitOfWorkAttribute), false)
            .Cast<UnitOfWorkAttribute>()
            .FirstOrDefault();

        // Assert
        attribute.ShouldNotBeNull();
        attribute.IsEnabled.ShouldBeFalse();
        // AutoCommit and AutoRollback remain at default values
        attribute.AutoCommit.ShouldBeTrue();
        attribute.AutoRollback.ShouldBeTrue();
    }

    [Fact]
    public void Attribute_IsInherited_ShouldWorkOnDerivedClass()
    {
        // Arrange
        var derivedCommandType = typeof(DerivedTestCommand);

        // Act
        var attribute = derivedCommandType.GetCustomAttributes(typeof(UnitOfWorkAttribute), true)
            .Cast<UnitOfWorkAttribute>()
            .FirstOrDefault();

        // Assert
        attribute.ShouldNotBeNull();
        attribute.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Attribute_MultipleApplication_ShouldNotBeAllowed()
    {
        // Arrange
        var attributeType = typeof(UnitOfWorkAttribute);
        var usageAttribute = attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        usageAttribute.ShouldNotBeNull();
        usageAttribute.AllowMultiple.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WithParameterizedIsEnabled_SetsPropertyCorrectly()
    {
        // Act
        var enabledAttribute = new UnitOfWorkAttribute(true);
        var disabledAttribute = new UnitOfWorkAttribute(false);

        // Assert
        enabledAttribute.IsEnabled.ShouldBeTrue();
        disabledAttribute.IsEnabled.ShouldBeFalse();
    }

    // Test helper classes
    [UnitOfWork]
    private class TestCommand
    {
    }

    [UnitOfWork(isEnabled: false)]
    private class TestCommandWithCustomValues
    {
    }

    [UnitOfWork]
    private class DerivedTestCommand : TestCommand
    {
    }
}
