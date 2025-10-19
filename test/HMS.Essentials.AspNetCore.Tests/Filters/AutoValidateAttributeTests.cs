using FluentValidation;
using HMS.Essentials.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HMS.Essentials.AspNetCore.Tests.Filters;

public class AutoValidateAttributeTests
{
    [Fact]
    public void AutoValidateAttribute_ShouldHaveCorrectAttributeUsage()
    {
        // Arrange & Act
        var attributeUsage = typeof(AutoValidateAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Class));
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Method));
        Assert.False(attributeUsage.AllowMultiple);
        Assert.True(attributeUsage.Inherited);
    }

    [Fact]
    public void IsReusable_ShouldReturnFalse()
    {
        // Arrange
        var attribute = new AutoValidateAttribute();

        // Act
        var isReusable = attribute.IsReusable;

        // Assert
        Assert.False(isReusable);
    }

    [Fact]
    public void CreateInstance_WithValidServiceProvider_ShouldReturnFluentValidationActionFilter()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockLogger = new Mock<ILogger<FluentValidationActionFilter>>();
        services.AddSingleton(mockLogger.Object);
        var serviceProvider = services.BuildServiceProvider();

        var attribute = new AutoValidateAttribute();

        // Act
        var result = attribute.CreateInstance(serviceProvider);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FluentValidationActionFilter>(result);
    }

    [Fact]
    public void CreateInstance_WithoutLogger_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var attribute = new AutoValidateAttribute();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => attribute.CreateInstance(serviceProvider));
        Assert.Contains("ILogger<FluentValidationActionFilter>", exception.Message);
        Assert.Contains("could not be resolved", exception.Message);
    }

    [Fact]
    public void CreateInstance_ShouldCreateNewInstanceEachTime()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockLogger = new Mock<ILogger<FluentValidationActionFilter>>();
        services.AddSingleton(mockLogger.Object);
        var serviceProvider = services.BuildServiceProvider();

        var attribute = new AutoValidateAttribute();

        // Act
        var instance1 = attribute.CreateInstance(serviceProvider);
        var instance2 = attribute.CreateInstance(serviceProvider);

        // Assert
        Assert.NotNull(instance1);
        Assert.NotNull(instance2);
        Assert.NotSame(instance1, instance2);
    }

    [Fact]
    public void AutoValidateAttribute_ImplementsIFilterFactory()
    {
        // Arrange & Act
        var attribute = new AutoValidateAttribute();

        // Assert
        Assert.IsAssignableFrom<IFilterFactory>(attribute);
    }

    [Fact]
    public void AutoValidateAttribute_InheritsFromAttribute()
    {
        // Arrange & Act
        var attribute = new AutoValidateAttribute();

        // Assert
        Assert.IsAssignableFrom<Attribute>(attribute);
    }

    [Fact]
    public void CreateInstance_WithLoggerFromDI_ShouldUseResolvedLogger()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();

        var attribute = new AutoValidateAttribute();

        // Act
        var result = attribute.CreateInstance(serviceProvider);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FluentValidationActionFilter>(result);
    }

    [Fact]
    public void AutoValidateAttribute_CanBeAppliedToClass()
    {
        // Arrange & Act
        var type = typeof(TestControllerWithAttribute);
        var attributes = type.GetCustomAttributes(typeof(AutoValidateAttribute), false);

        // Assert
        Assert.Single(attributes);
        Assert.IsType<AutoValidateAttribute>(attributes[0]);
    }

    [Fact]
    public void AutoValidateAttribute_CanBeAppliedToMethod()
    {
        // Arrange & Act
        var method = typeof(TestControllerWithoutAttribute).GetMethod(nameof(TestControllerWithoutAttribute.ActionWithAttribute));
        var attributes = method?.GetCustomAttributes(typeof(AutoValidateAttribute), false);

        // Assert
        Assert.NotNull(attributes);
        Assert.Single(attributes);
        Assert.IsType<AutoValidateAttribute>(attributes[0]);
    }

    [Fact]
    public void AutoValidateAttribute_CannotBeAppliedMultipleTimes()
    {
        // Arrange & Act
        var attributeUsage = typeof(AutoValidateAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attributeUsage);
        Assert.False(attributeUsage.AllowMultiple);
    }

    [Fact]
    public void AutoValidateAttribute_IsInheritedByDerivedClasses()
    {
        // Arrange & Act
        var attributeUsage = typeof(AutoValidateAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.Inherited);
    }

    [Fact]
    public void CreateInstance_ReturnsIFilterMetadata()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockLogger = new Mock<ILogger<FluentValidationActionFilter>>();
        services.AddSingleton(mockLogger.Object);
        var serviceProvider = services.BuildServiceProvider();

        var attribute = new AutoValidateAttribute();

        // Act
        var result = attribute.CreateInstance(serviceProvider);

        // Assert
        Assert.IsAssignableFrom<IFilterMetadata>(result);
    }

    [Fact]
    public void CreateInstance_WithMultipleCalls_ShouldNotCache()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var attribute = new AutoValidateAttribute();

        // Act
        var instance1 = attribute.CreateInstance(serviceProvider);
        var instance2 = attribute.CreateInstance(serviceProvider);
        var instance3 = attribute.CreateInstance(serviceProvider);

        // Assert
        Assert.NotSame(instance1, instance2);
        Assert.NotSame(instance2, instance3);
        Assert.NotSame(instance1, instance3);
    }

    [Fact]
    public void AutoValidateAttribute_DefaultConstructor_ShouldNotThrow()
    {
        // Arrange & Act
        var exception = Record.Exception(() => new AutoValidateAttribute());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CreateInstance_WithDisposedServiceProvider_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.Dispose();

        var attribute = new AutoValidateAttribute();

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => attribute.CreateInstance(serviceProvider));
    }

    // Test helper classes
    [AutoValidate]
    private class TestControllerWithAttribute
    {
        public void TestAction() { }
    }

    private class TestControllerWithoutAttribute
    {
        [AutoValidate]
        public void ActionWithAttribute() { }

        public void ActionWithoutAttribute() { }
    }
}
