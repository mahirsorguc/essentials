using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using HMS.Essentials.MediatR.Behaviors;

namespace HMS.Essentials.MediatR.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<ILogger<ValidationBehavior<TestRequest, string>>> _mockLogger;

    public ValidationBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<ValidationBehavior<TestRequest, string>>>();
    }

    [Fact]
    public async Task Handle_WithNoValidators_ShouldContinueExecution()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators, _mockLogger.Object);
        var request = new TestRequest();
        var expectedResponse = "test response";
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldContinueExecution()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new[] { mockValidator.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators, _mockLogger.Object);
        var request = new TestRequest();
        var expectedResponse = "test response";
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("PropertyName", "Error message")
        };

        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var validators = new[] { mockValidator.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators, _mockLogger.Object);
        var request = new TestRequest();
        Task<string> Next(CancellationToken ct) => Task.FromResult("test response");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(
            async () => await behavior.Handle(request, Next, CancellationToken.None));

        exception.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_ShouldCollectAllErrors()
    {
        // Arrange
        var mockValidator1 = new Mock<IValidator<TestRequest>>();
        mockValidator1
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Field1", "Error 1") }));

        var mockValidator2 = new Mock<IValidator<TestRequest>>();
        mockValidator2
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Field2", "Error 2") }));

        var validators = new[] { mockValidator1.Object, mockValidator2.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators, _mockLogger.Object);
        var request = new TestRequest();
        Task<string> Next(CancellationToken ct) => Task.FromResult("test response");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(
            async () => await behavior.Handle(request, Next, CancellationToken.None));

        exception.Errors.Count().ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldLogWarning()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("PropertyName", "Error message")
        };

        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var validators = new[] { mockValidator.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators, _mockLogger.Object);
        var request = new TestRequest();
        Task<string> Next(CancellationToken ct) => Task.FromResult("test response");

        // Act
        try
        {
            await behavior.Handle(request, Next, CancellationToken.None);
        }
        catch (ValidationException)
        {
            // Expected
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullValidators_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new ValidationBehavior<TestRequest, string>(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new ValidationBehavior<TestRequest, string>(validators, null!));
    }

    public class TestRequest : IRequest<string>
    {
    }
}
