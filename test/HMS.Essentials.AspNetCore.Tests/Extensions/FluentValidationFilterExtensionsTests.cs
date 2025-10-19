using FluentValidation;
using HMS.Essentials.AspNetCore.Extensions;
using HMS.Essentials.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HMS.Essentials.AspNetCore.Tests.Extensions;

public class FluentValidationFilterExtensionsTests
{
    [Fact]
    public void AddFluentValidationAutoValidation_WithMvcBuilder_ShouldAddFilter()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var mvcBuilder = services.AddControllers();

        // Act
        var result = mvcBuilder.AddFluentValidationAutoValidation();

        // Assert
        Assert.NotNull(result);
        Assert.Same(mvcBuilder, result);

        var serviceProvider = services.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MvcOptions>>();
        
        Assert.NotEmpty(mvcOptions.Value.Filters);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithNullMvcBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange
        IMvcBuilder builder = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddFluentValidationAutoValidation());
        Assert.Equal("builder", exception.ParamName);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithMvcOptions_ShouldAddFilter()
    {
        // Arrange
        var options = new MvcOptions();

        // Act
        var result = options.AddFluentValidationAutoValidation();

        // Assert
        Assert.NotNull(result);
        Assert.Same(options, result);
        Assert.Single(options.Filters);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithNullMvcOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        MvcOptions options = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => options.AddFluentValidationAutoValidation());
        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithMvcBuilderAndOrder_ShouldAddFilterWithOrder()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var mvcBuilder = services.AddControllers();
        const int expectedOrder = 10;

        // Act
        var result = mvcBuilder.AddFluentValidationAutoValidation(expectedOrder);

        // Assert
        Assert.NotNull(result);
        Assert.Same(mvcBuilder, result);

        var serviceProvider = services.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MvcOptions>>();
        
        Assert.NotEmpty(mvcOptions.Value.Filters);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithNullMvcBuilderAndOrder_ShouldThrowArgumentNullException()
    {
        // Arrange
        IMvcBuilder builder = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddFluentValidationAutoValidation(10));
        Assert.Equal("builder", exception.ParamName);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithMvcOptionsAndOrder_ShouldAddFilterWithOrder()
    {
        // Arrange
        var options = new MvcOptions();
        const int expectedOrder = 10;

        // Act
        var result = options.AddFluentValidationAutoValidation(expectedOrder);

        // Assert
        Assert.NotNull(result);
        Assert.Same(options, result);
        Assert.Single(options.Filters);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithNullMvcOptionsAndOrder_ShouldThrowArgumentNullException()
    {
        // Arrange
        MvcOptions options = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => options.AddFluentValidationAutoValidation(10));
        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_MultipleCallsWithMvcBuilder_ShouldAddMultipleFilters()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var mvcBuilder = services.AddControllers();

        // Act
        mvcBuilder.AddFluentValidationAutoValidation();
        mvcBuilder.AddFluentValidationAutoValidation(order: 5);
        mvcBuilder.AddFluentValidationAutoValidation(order: 10);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MvcOptions>>();
        
        // Should have multiple filters added
        Assert.True(mvcOptions.Value.Filters.Count >= 3);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_MultipleCallsWithMvcOptions_ShouldAddMultipleFilters()
    {
        // Arrange
        var options = new MvcOptions();

        // Act
        options.AddFluentValidationAutoValidation();
        options.AddFluentValidationAutoValidation(order: 5);
        options.AddFluentValidationAutoValidation(order: 10);

        // Assert
        Assert.Equal(3, options.Filters.Count);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithDifferentOrders_ShouldMaintainOrder()
    {
        // Arrange
        var options = new MvcOptions();

        // Act
        options.AddFluentValidationAutoValidation(order: 10);
        options.AddFluentValidationAutoValidation(order: 5);
        options.AddFluentValidationAutoValidation(order: 15);

        // Assert
        Assert.Equal(3, options.Filters.Count);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_ShouldWorkWithExistingFilters()
    {
        // Arrange
        var options = new MvcOptions();
        options.Filters.Add(new ConsumesAttribute("application/json"));

        // Act
        options.AddFluentValidationAutoValidation();

        // Assert
        Assert.Equal(2, options.Filters.Count);
        Assert.IsType<ConsumesAttribute>(options.Filters[0]);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var mvcBuilder = services.AddControllers();

        // Act
        var result1 = mvcBuilder.AddFluentValidationAutoValidation();
        var result2 = result1.AddFluentValidationAutoValidation(order: 5);

        // Assert
        Assert.Same(mvcBuilder, result1);
        Assert.Same(mvcBuilder, result2);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_ShouldReturnSameMvcOptionsInstance()
    {
        // Arrange
        var options = new MvcOptions();

        // Act
        var result1 = options.AddFluentValidationAutoValidation();
        var result2 = result1.AddFluentValidationAutoValidation(order: 5);

        // Assert
        Assert.Same(options, result1);
        Assert.Same(options, result2);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithZeroOrder_ShouldWork()
    {
        // Arrange
        var options = new MvcOptions();

        // Act
        var result = options.AddFluentValidationAutoValidation(order: 0);

        // Assert
        Assert.NotNull(result);
        Assert.Single(options.Filters);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_WithNegativeOrder_ShouldWork()
    {
        // Arrange
        var options = new MvcOptions();

        // Act
        var result = options.AddFluentValidationAutoValidation(order: -10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(options.Filters);
    }

    [Fact]
    public void AddFluentValidationAutoValidation_MethodChaining_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var result = services
            .AddControllers()
            .AddFluentValidationAutoValidation()
            .AddFluentValidationAutoValidation(order: 5);

        // Assert
        Assert.NotNull(result);
        var serviceProvider = services.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MvcOptions>>();
        Assert.True(mvcOptions.Value.Filters.Count >= 2);
    }
}
