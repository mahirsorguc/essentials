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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting new transaction")),
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

    [Fact]
    public async Task Handle_WithoutUnitOfWorkAttribute_ShouldSkipTransactionManagement()
    {
        // Arrange
        var command = new CommandWithoutAttribute();
        var expectedResponse = "test response";
        var mockLogger = new Mock<ILogger<UnitOfWorkBehavior<CommandWithoutAttribute, string>>>();
        var behavior = new UnitOfWorkBehavior<CommandWithoutAttribute, string>(_mockUnitOfWork.Object, mockLogger.Object);
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDisabledUnitOfWorkAttribute_ShouldSkipTransactionManagement()
    {
        // Arrange
        var command = new CommandWithDisabledUnitOfWork();
        var expectedResponse = "test response";
        var mockLogger = new Mock<ILogger<UnitOfWorkBehavior<CommandWithDisabledUnitOfWork, string>>>();
        var behavior = new UnitOfWorkBehavior<CommandWithDisabledUnitOfWork, string>(_mockUnitOfWork.Object, mockLogger.Object);
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithAutoCommitDisabled_ShouldNotCommitTransaction()
    {
        // Arrange
        var command = new CommandWithoutAutoCommit();
        var expectedResponse = "test response";
        var mockLogger = new Mock<ILogger<UnitOfWorkBehavior<CommandWithoutAutoCommit, string>>>();
        var behavior = new UnitOfWorkBehavior<CommandWithoutAutoCommit, string>(_mockUnitOfWork.Object, mockLogger.Object);
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithAutoRollbackDisabled_ShouldNotRollbackOnException()
    {
        // Arrange
        var command = new CommandWithoutAutoRollback();
        var expectedException = new InvalidOperationException("Test exception");
        var mockLogger = new Mock<ILogger<UnitOfWorkBehavior<CommandWithoutAutoRollback, string>>>();
        var behavior = new UnitOfWorkBehavior<CommandWithoutAutoRollback, string>(_mockUnitOfWork.Object, mockLogger.Object);
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await behavior.Handle(command, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithActiveTransaction_ShouldNotStartNewTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResponse = "test response";
        _mockUnitOfWork.Setup(u => u.HasActiveTransaction).Returns(true);
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithActiveTransaction_ShouldLogParticipationMessage()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResponse = "test response";
        _mockUnitOfWork.Setup(u => u.HasActiveTransaction).Returns(true);
        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act
        await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Transaction already active") && v.ToString()!.Contains("participating in existing transaction")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithActiveTransactionAndException_ShouldNotRollback()
    {
        // Arrange
        var command = new TestCommand();
        var expectedException = new InvalidOperationException("Test exception");
        _mockUnitOfWork.Setup(u => u.HasActiveTransaction).Returns(true);
        Task<string> NextThrows(CancellationToken ct) => throw expectedException;

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _behavior.Handle(command, NextThrows, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NestedCommand_ShouldParticipateInParentTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResponse = "test response";
        
        // Simulate nested transaction scenario:
        // First call: No active transaction (outer command)
        // Second call: Active transaction (inner/nested command)
        var setupSequence = _mockUnitOfWork.SetupSequence(u => u.HasActiveTransaction)
            .Returns(false)  // First command - no active transaction
            .Returns(true);  // Nested command - transaction already active

        Task<string> Next(CancellationToken ct) => Task.FromResult(expectedResponse);

        // Act - First command (outer)
        var result1 = await _behavior.Handle(command, Next, CancellationToken.None);
        
        // Act - Nested command (inner)
        var result2 = await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result1.ShouldBe(expectedResponse);
        result2.ShouldBe(expectedResponse);
        
        // Transaction should only be started once (by the outer command)
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Transaction should only be committed once (by the outer command)
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [UnitOfWork]
    public class TestCommand : ICommand<string>
    {
    }

    public class CommandWithoutAttribute : ICommand<string>
    {
    }

    [UnitOfWork(false)]
    public class CommandWithDisabledUnitOfWork : ICommand<string>
    {
    }

    [UnitOfWork(AutoCommit = false)]
    public class CommandWithoutAutoCommit : ICommand<string>
    {
    }

    [UnitOfWork(AutoRollback = false)]
    public class CommandWithoutAutoRollback : ICommand<string>
    {
    }
}
