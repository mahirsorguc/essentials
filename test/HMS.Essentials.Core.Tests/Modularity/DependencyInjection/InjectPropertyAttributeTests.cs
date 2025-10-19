using System.Reflection;
using HMS.Essentials.Modularity.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.Modularity.Tests.DependencyInjection;

/// <summary>
/// Tests for <see cref="InjectPropertyAttribute"/>.
/// </summary>
public class InjectPropertyAttributeTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var attribute = new InjectPropertyAttribute();

        // Assert
        attribute.Required.ShouldBeTrue();
        attribute.ServiceKey.ShouldBeNull();
    }

    [Fact]
    public void Required_ShouldBeSettable()
    {
        // Arrange
        var attribute = new InjectPropertyAttribute();

        // Act
        attribute.Required = false;

        // Assert
        attribute.Required.ShouldBeFalse();
    }

    [Fact]
    public void ServiceKey_ShouldBeSettable()
    {
        // Arrange
        var attribute = new InjectPropertyAttribute();
        var key = "myServiceKey";

        // Act
        attribute.ServiceKey = key;

        // Assert
        attribute.ServiceKey.ShouldBe(key);
    }

    [Fact]
    public void Attribute_ShouldBeApplicableToProperties()
    {
        // Arrange
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;

        // Act
        var attribute = propertyInfo.GetCustomAttributes(typeof(InjectPropertyAttribute), false).FirstOrDefault();

        // Assert
        attribute.ShouldNotBeNull();
        attribute.ShouldBeOfType<InjectPropertyAttribute>();
    }

    [Fact]
    public void Attribute_ShouldNotAllowMultiple()
    {
        // Arrange
        var attributeUsage = typeof(InjectPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.AllowMultiple.ShouldBeFalse();
    }

    [Fact]
    public void Attribute_ShouldBeInherited()
    {
        // Arrange
        var attributeUsage = typeof(InjectPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.Inherited.ShouldBeTrue();
    }

    [Fact]
    public void Attribute_ShouldOnlyApplyToProperties()
    {
        // Arrange
        var attributeUsage = typeof(InjectPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.ValidOn.ShouldBe(AttributeTargets.Property);
    }

    [Fact]
    public void Attribute_ShouldBeInheritedByDerivedClass()
    {
        // Arrange
        var basePropertyInfo = typeof(BaseClass).GetProperty(nameof(BaseClass.BaseProperty))!;
        var derivedPropertyInfo = typeof(DerivedClass).GetProperty(nameof(DerivedClass.BaseProperty))!;

        // Act
        var baseAttribute = basePropertyInfo.GetCustomAttribute<InjectPropertyAttribute>();
        var derivedAttribute = derivedPropertyInfo.GetCustomAttribute<InjectPropertyAttribute>(true);

        // Assert
        baseAttribute.ShouldNotBeNull();
        derivedAttribute.ShouldNotBeNull();
    }

    // Test helper classes
    private class TestClass
    {
        [InjectProperty]
        public object? TestProperty { get; set; }
    }

    private class BaseClass
    {
        [InjectProperty]
        public virtual object? BaseProperty { get; set; }
    }

    private class DerivedClass : BaseClass
    {
        public override object? BaseProperty { get; set; }
    }
}
