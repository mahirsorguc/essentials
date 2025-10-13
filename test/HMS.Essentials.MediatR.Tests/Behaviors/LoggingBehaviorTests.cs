using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using HMS.Essentials.MediatR.Behaviors;

namespace HMS.Essentials.MediatR.Tests.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, string>>> _mockLogger;
    private readonly LoggingBehavior<TestRequest, string> _behavior;

    public LoggingBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<LoggingBehavior<TestRequest, string>>>();
        _behavior = new LoggingBehavior<TestRequest, string>(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogBeforeAndAfterExecution()
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
        
        // Verify logging calls
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling TestRequest")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handled TestRequest successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldLogError()
    {
        // Arrange
        var request = new TestRequest();
        var expectedException = new InvalidOperationException("Test exception");
        
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _behavior.Handle(request, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error handling TestRequest")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new LoggingBehavior<TestRequest, string>(null!));
    }

    public class TestRequest : IRequest<string>
    {
    }
}
