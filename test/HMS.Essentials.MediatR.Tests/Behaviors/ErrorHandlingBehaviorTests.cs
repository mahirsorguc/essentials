using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using HMS.Essentials.MediatR.Behaviors;

namespace HMS.Essentials.MediatR.Tests.Behaviors;

public class ErrorHandlingBehaviorTests
{
    private readonly Mock<ILogger<ErrorHandlingBehavior<TestRequest, string>>> _mockLogger;
    private readonly Mock<IOptions<EssentialsMediatROptions>> _mockOptions;
    private readonly ErrorHandlingBehavior<TestRequest, string> _behavior;

    public ErrorHandlingBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<ErrorHandlingBehavior<TestRequest, string>>>();
        _mockOptions = new Mock<IOptions<EssentialsMediatROptions>>();
        _mockOptions.Setup(x => x.Value).Returns(new EssentialsMediatROptions
        {
            ErrorHandlingEnabled = true
        });
        _behavior = new ErrorHandlingBehavior<TestRequest, string>(_mockLogger.Object, _mockOptions.Object);
    }

    [Fact]
    public async Task Handle_WithSuccessfulExecution_ShouldReturnResponse()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResponse = "test response";
        var nextWasCalled = false;
        
        Task<string> Next(CancellationToken ct)
        {
            nextWasCalled = true;
            return Task.FromResult(expectedResponse);
        }

        // Act
        var result = await _behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        nextWasCalled.ShouldBeTrue();
        
        // Should not log errors on success
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var request = new TestRequest();
        var expectedException = new InvalidOperationException("Test exception");
        
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _behavior.Handle(request, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);

        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception occurred while processing TestRequest")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentExceptionTypes_ShouldLogCorrectExceptionType()
    {
        // Arrange
        var request = new TestRequest();
        var expectedException = new ArgumentNullException("testParam", "Test argument null exception");
        
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentNullException>(
            async () => await _behavior.Handle(request, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);

        // Verify exception type is logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ArgumentNullException")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDisabled_ShouldNotCatchExceptions()
    {
        // Arrange
        var mockOptionsDisabled = new Mock<IOptions<EssentialsMediatROptions>>();
        mockOptionsDisabled.Setup(x => x.Value).Returns(new EssentialsMediatROptions
        {
            ErrorHandlingEnabled = false
        });
        var behaviorDisabled = new ErrorHandlingBehavior<TestRequest, string>(_mockLogger.Object, mockOptionsDisabled.Object);

        var request = new TestRequest();
        var expectedException = new InvalidOperationException("Test exception");
        
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await behaviorDisabled.Handle(request, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);

        // Should not log when disabled
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDisabled_ShouldReturnSuccessResponse()
    {
        // Arrange
        var mockOptionsDisabled = new Mock<IOptions<EssentialsMediatROptions>>();
        mockOptionsDisabled.Setup(x => x.Value).Returns(new EssentialsMediatROptions
        {
            ErrorHandlingEnabled = false
        });
        var behaviorDisabled = new ErrorHandlingBehavior<TestRequest, string>(_mockLogger.Object, mockOptionsDisabled.Object);

        var request = new TestRequest();
        var expectedResponse = "test response";
        
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behaviorDisabled.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithNestedExceptions_ShouldLogOuterException()
    {
        // Arrange
        var request = new TestRequest();
        var innerException = new ArgumentException("Inner exception");
        var outerException = new InvalidOperationException("Outer exception", innerException);
        
        Task<string> NextThrows(CancellationToken ct) => throw outerException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _behavior.Handle(request, NextThrows, CancellationToken.None));

        exception.ShouldBe(outerException);
        exception.InnerException.ShouldBe(innerException);

        // Verify outer exception is logged (with inner exception information)
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Outer exception")),
                outerException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new ErrorHandlingBehavior<TestRequest, string>(null!, _mockOptions.Object));
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new ErrorHandlingBehavior<TestRequest, string>(_mockLogger.Object, null!));
    }

    [Fact]
    public async Task Handle_WithCancellationException_ShouldLogAndRethrow()
    {
        // Arrange
        var request = new TestRequest();
        var expectedException = new TaskCanceledException("Operation was cancelled");
        
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        await Should.ThrowAsync<TaskCanceledException>(
            async () => await _behavior.Handle(request, NextThrows, CancellationToken.None));

        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public class TestRequest : IRequest<string>
    {
    }
}
