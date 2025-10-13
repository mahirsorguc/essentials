using FluentAssertions;

namespace HMS.Essentials.UnitOfWork.InMemoryDummy;

public class InMemoryUnitOfWorkTests
{
    [Fact]
    public void HasActiveTransaction_InitiallyFalse()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task BeginTransactionAsync_SetsActiveTransaction()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act
        await unitOfWork.BeginTransactionAsync();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeTrue();
    }

    [Fact]
    public async Task BeginTransactionAsync_ThrowsWhenTransactionAlreadyActive()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();
        await unitOfWork.BeginTransactionAsync();

        // Act
        var act = async () => await unitOfWork.BeginTransactionAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A transaction is already active.");
    }

    [Fact]
    public async Task CommitTransactionAsync_ClearsActiveTransaction()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();
        await unitOfWork.BeginTransactionAsync();

        // Act
        await unitOfWork.CommitTransactionAsync();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task CommitTransactionAsync_ThrowsWhenNoActiveTransaction()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act
        var act = async () => await unitOfWork.CommitTransactionAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No active transaction to commit.");
    }

    [Fact]
    public async Task RollbackTransactionAsync_ClearsActiveTransaction()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();
        await unitOfWork.BeginTransactionAsync();

        // Act
        await unitOfWork.RollbackTransactionAsync();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task RollbackTransactionAsync_ThrowsWhenNoActiveTransaction()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act
        var act = async () => await unitOfWork.RollbackTransactionAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No active transaction to rollback.");
    }

    [Fact]
    public async Task SaveChangesAsync_ReturnsZero_InMemoryImplementation()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_SupportsCancellation()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();
        using var cts = new CancellationTokenSource();

        // Act
        var result = await unitOfWork.SaveChangesAsync(cts.Token);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task FullTransactionLifecycle_WorksCorrectly()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act & Assert - Begin
        await unitOfWork.BeginTransactionAsync();
        unitOfWork.HasActiveTransaction.Should().BeTrue();

        // Act & Assert - Save
        var saveResult = await unitOfWork.SaveChangesAsync();
        saveResult.Should().Be(0);
        unitOfWork.HasActiveTransaction.Should().BeTrue();

        // Act & Assert - Commit
        await unitOfWork.CommitTransactionAsync();
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task TransactionRollback_AfterBegin_WorksCorrectly()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act
        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.RollbackTransactionAsync();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task Dispose_RollsBackActiveTransaction()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();
        await unitOfWork.BeginTransactionAsync();

        // Act
        unitOfWork.Dispose();

        // Assert - No exception should be thrown
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();

        // Act & Assert - No exception should be thrown
        unitOfWork.Dispose();
        unitOfWork.Dispose();
    }

    [Fact]
    public async Task MultipleOperationsInTransaction_WorkCorrectly()
    {
        // Arrange
        using var unitOfWork = new InMemoryUnitOfWork();

        // Act
        await unitOfWork.BeginTransactionAsync();

        for (int i = 0; i < 5; i++)
        {
            await unitOfWork.SaveChangesAsync();
        }

        await unitOfWork.CommitTransactionAsync();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }
}
