using HMS.Essentials.MediatR.Behaviors;
using HMS.Essentials.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace HMS.Essentials.MediatR.Tests.Behaviors;

public class UnitOfWorkBehaviorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<UnitOfWorkBehavior<TestCommand, string>>> _mockLogger;
    private readonly UnitOfWorkBehavior<TestCommand, string> _behavior;

    public UnitOfWorkBehaviorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<UnitOfWorkBehavior<TestCommand, string>>>();
        _behavior = new UnitOfWorkBehavior<TestCommand, string>(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithSuccessfulExecution_ShouldCommitTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResponse = "test response";
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithException_ShouldRollbackTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var expectedException = new InvalidOperationException("Test exception");
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _behavior.Handle(command, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogDebugMessages()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResponse = "test response";
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting transaction")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Transaction committed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithException_ShouldLogError()
    {
        // Arrange
        var command = new TestCommand();
        var expectedException = new InvalidOperationException("Test exception");
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act
        try
        {
            await _behavior.Handle(command, NextThrows, CancellationToken.None);
        }
        catch
        {
            // Expected
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Transaction failed")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullUnitOfWork_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new UnitOfWorkBehavior<TestCommand, string>(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new UnitOfWorkBehavior<TestCommand, string>(_mockUnitOfWork.Object, null!));
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassToUnitOfWork()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResponse = "test response";
        var cancellationToken = new CancellationToken();
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        await _behavior.Handle(command, Next, cancellationToken);

        // Assert
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(cancellationToken), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(cancellationToken), Times.Once);
    }

    public class TestCommand : ICommand<string>
    {
    }
}
