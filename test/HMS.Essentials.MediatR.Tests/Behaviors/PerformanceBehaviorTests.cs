using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using HMS.Essentials.MediatR.Behaviors;

namespace HMS.Essentials.MediatR.Tests.Behaviors;

public class PerformanceBehaviorTests
{
    private readonly Mock<ILogger<PerformanceBehavior<TestRequest, string>>> _mockLogger;
    private readonly PerformanceBehavior<TestRequest, string> _behavior;

    public PerformanceBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<PerformanceBehavior<TestRequest, string>>>();
        _behavior = new PerformanceBehavior<TestRequest, string>(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenExecutionIsFast_ShouldNotLogWarning()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResponse = "test response";
        
        async Task<string> Next(CancellationToken ct)
        {
            await Task.Delay(50, ct); // Fast execution
            return expectedResponse;
        }

        // Act
        var result = await _behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        
        // Verify no warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenExecutionIsSlow_ShouldLogWarning()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResponse = "test response";
        
        async Task<string> Next(CancellationToken ct)
        {
            await Task.Delay(600, ct); // Slow execution (>500ms)
            return expectedResponse;
        }

        // Act
        var result = await _behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Long Running Request")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new PerformanceBehavior<TestRequest, string>(null!));
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectResponse()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResponse = "expected response";
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await _behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    public class TestRequest : IRequest<string>
    {
    }
}
