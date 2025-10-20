using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace HMS.Essentials.AspNetCore.Filters;

public class FluentValidationActionFilterTests
{
    private readonly Mock<ILogger<FluentValidationActionFilter>> _mockLogger;
    private readonly FluentValidationActionFilter _filter;

    public FluentValidationActionFilterTests()
    {
        _mockLogger = new Mock<ILogger<FluentValidationActionFilter>>();
        _filter = new FluentValidationActionFilter(_mockLogger.Object);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithValidModel_ShouldContinueExecution()
    {
        // Arrange
        var dto = new TestDto { Name = "Valid Name", Age = 25 };
        var validator = new TestDtoValidator();
        
        var context = CreateActionExecutingContext(dto, validator);
        var nextCalled = false;
        
        Task<ActionExecutedContext> Next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Null(context.Result);
        Assert.True(context.ModelState.IsValid);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithInvalidModel_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new TestDto { Name = "", Age = -1 }; // Invalid
        var validator = new TestDtoValidator();
        
        var context = CreateActionExecutingContext(dto, validator);
        var nextCalled = false;
        
        Task<ActionExecutedContext> Next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        Assert.False(nextCalled);
        Assert.NotNull(context.Result);
        Assert.IsType<BadRequestObjectResult>(context.Result);
        Assert.False(context.ModelState.IsValid);

        var badRequestResult = context.Result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithNoValidator_ShouldContinueExecution()
    {
        // Arrange
        var dto = new TestDto { Name = "Test", Age = 25 };
        
        // Create context without validator
        var context = CreateActionExecutingContext(dto, null);
        var nextCalled = false;
        
        Task<ActionExecutedContext> Next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Null(context.Result);
        Assert.True(context.ModelState.IsValid);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithNullParameter_ShouldContinueExecution()
    {
        // Arrange
        var validator = new TestDtoValidator();
        var context = CreateActionExecutingContext(null, validator);
        var nextCalled = false;
        
        Task<ActionExecutedContext> Next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithAlreadyInvalidModelState_ShouldSkipValidation()
    {
        // Arrange
        var dto = new TestDto { Name = "", Age = -1 }; // Invalid
        var validator = new TestDtoValidator();
        var context = CreateActionExecutingContext(dto, validator);
        
        // Make ModelState invalid before filter runs
        context.ModelState.AddModelError("Test", "Pre-existing error");
        
        var nextCalled = false;
        
        Task<ActionExecutedContext> Next()
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        Assert.True(nextCalled); // Should continue even though dto is invalid
        Assert.False(context.ModelState.IsValid);
        Assert.Single(context.ModelState); // Only the pre-existing error
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var dto = new TestDto { Name = "", Age = -1 }; // Both fields invalid
        var validator = new TestDtoValidator();
        var context = CreateActionExecutingContext(dto, validator);
        
        Task<ActionExecutedContext> Next()
        {
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        var badRequestResult = context.Result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        
        Assert.False(context.ModelState.IsValid);
        Assert.True(context.ModelState.ErrorCount >= 2);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithValidationError_ShouldIncludePropertyName()
    {
        // Arrange
        var dto = new TestDto { Name = "", Age = 25 }; // Name invalid
        var validator = new TestDtoValidator();
        var context = CreateActionExecutingContext(dto, validator);
        
        Task<ActionExecutedContext> Next()
        {
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        Assert.False(context.ModelState.IsValid);
        Assert.True(context.ModelState.ContainsKey("dto.Name"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_ShouldReturnRfc7807ProblemDetails()
    {
        // Arrange
        var dto = new TestDto { Name = "", Age = -1 };
        var validator = new TestDtoValidator();
        var context = CreateActionExecutingContext(dto, validator);
        
        Task<ActionExecutedContext> Next()
        {
            return Task.FromResult(new ActionExecutedContext(
                context,
                new List<IFilterMetadata>(),
                context.Controller));
        }

        // Act
        await _filter.OnActionExecutionAsync(context, Next);

        // Assert
        var badRequestResult = context.Result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        
        var value = badRequestResult.Value;
        Assert.NotNull(value);
        
        var properties = value.GetType().GetProperties();
        Assert.Contains(properties, p => p.Name == "Type");
        Assert.Contains(properties, p => p.Name == "Title");
        Assert.Contains(properties, p => p.Name == "Status");
        Assert.Contains(properties, p => p.Name == "Errors");
        Assert.Contains(properties, p => p.Name == "TraceId");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FluentValidationActionFilter(null!));
    }

    private ActionExecutingContext CreateActionExecutingContext(
        object? parameterValue,
        IValidator? validator)
    {
        var services = new ServiceCollection();
        
        if (validator != null && parameterValue != null)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(parameterValue.GetType());
            services.AddSingleton(validatorType, validator);
        }
        
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            TraceIdentifier = "test-trace-id"
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor
            {
                Parameters = new List<ParameterDescriptor>
                {
                    new ParameterDescriptor
                    {
                        Name = "dto",
                        ParameterType = parameterValue?.GetType() ?? typeof(object)
                    }
                }
            },
            new ModelStateDictionary());

        var actionArguments = new Dictionary<string, object?>();
        if (parameterValue != null)
        {
            actionArguments["dto"] = parameterValue;
        }

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            actionArguments,
            controller: new object());
    }

    // Test DTO
    public class TestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    // Test Validator
    public class TestDtoValidator : AbstractValidator<TestDto>
    {
        public TestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Age)
                .GreaterThan(0).WithMessage("Age must be greater than 0");
        }
    }
}
