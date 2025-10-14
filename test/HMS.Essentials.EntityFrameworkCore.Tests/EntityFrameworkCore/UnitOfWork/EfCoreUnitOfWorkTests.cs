using FluentAssertions;
using HMS.Essentials.EntityFrameworkCore.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HMS.Essentials.EntityFrameworkCore.UnitOfWork;

public class EfCoreUnitOfWorkTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly EfCoreUnitOfWork<TestDbContext> _unitOfWork;

    public EfCoreUnitOfWorkTests()
    {
        _context = DbContextHelper.CreateSqliteInMemoryContext();
        _unitOfWork = new EfCoreUnitOfWork<TestDbContext>(_context);
    }

    public void Dispose()
    {
        _unitOfWork?.Dispose();
        _context?.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDbContext_ShouldCreateInstance()
    {
        // Arrange & Act
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(_context);

        // Assert
        unitOfWork.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullDbContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new EfCoreUnitOfWork<TestDbContext>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    [Fact]
    public void Constructor_WithLogger_ShouldCreateInstance()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EfCoreUnitOfWork<TestDbContext>>>();

        // Act
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(_context, loggerMock.Object);

        // Assert
        unitOfWork.Should().NotBeNull();
    }

    #endregion

    #region HasActiveTransaction Tests

    [Fact]
    public void HasActiveTransaction_InitiallyFalse()
    {
        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task HasActiveTransaction_AfterBeginTransaction_ShouldBeTrue()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeTrue();
    }

    [Fact]
    public async Task HasActiveTransaction_AfterCommit_ShouldBeFalse()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task HasActiveTransaction_AfterRollback_ShouldBeFalse()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    #endregion

    #region SaveChangesAsync Tests

    [Fact]
    public async Task SaveChangesAsync_WithChanges_ShouldReturnAffectedCount()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "Test Entity",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.TestEntities.Add(entity);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
    {
        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var entity = new TestEntity
        {
            Name = "Test Entity",
            CreatedAt = DateTime.UtcNow
        };
        _context.TestEntities.Add(entity);

        // Act
        var result = await _unitOfWork.SaveChangesAsync(cts.Token);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleChanges_ShouldReturnCorrectCount()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity { Name = "Entity 1", CreatedAt = DateTime.UtcNow },
            new TestEntity { Name = "Entity 2", CreatedAt = DateTime.UtcNow },
            new TestEntity { Name = "Entity 3", CreatedAt = DateTime.UtcNow }
        };
        _context.TestEntities.AddRange(entities);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(3);
    }

    #endregion

    #region BeginTransactionAsync Tests

    [Fact]
    public async Task BeginTransactionAsync_ShouldStartTransaction()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeTrue();
    }

    [Fact]
    public async Task BeginTransactionAsync_WithActiveTransaction_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        var act = async () => await _unitOfWork.BeginTransactionAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A transaction is already active.");
    }

    [Fact]
    public async Task BeginTransactionAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        await _unitOfWork.BeginTransactionAsync(cts.Token);

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeTrue();
    }

    #endregion

    #region CommitTransactionAsync Tests

    [Fact]
    public async Task CommitTransactionAsync_WithActiveTransaction_ShouldCommit()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);

        // Act
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
        var saved = await _context.TestEntities.FirstOrDefaultAsync(e => e.Name == "Test");
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutActiveTransaction_ShouldThrowInvalidOperationException()
    {
        // Act
        var act = async () => await _unitOfWork.CommitTransactionAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No active transaction to commit.");
    }

    [Fact]
    public async Task CommitTransactionAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.CommitTransactionAsync(cts.Token);

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task CommitTransactionAsync_SavesChangesBeforeCommit()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var entity = new TestEntity
        {
            Name = "Transaction Test",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.TestEntities.Add(entity);

        // Act
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(1);
    }

    #endregion

    #region RollbackTransactionAsync Tests

    [Fact]
    public async Task RollbackTransactionAsync_WithActiveTransaction_ShouldRollback()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);

        // Act
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutActiveTransaction_ShouldNotThrow()
    {
        // Act
        var act = async () => await _unitOfWork.RollbackTransactionAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.RollbackTransactionAsync(cts.Token);

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task RollbackTransactionAsync_ShouldDiscardChanges()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var entity = new TestEntity
        {
            Name = "Rollback Test",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(0);
    }

    #endregion

    #region GetDbContext Tests

    [Fact]
    public void GetDbContext_ShouldReturnDbContext()
    {
        // Act
        var result = _unitOfWork.GetDbContext();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(_context);
    }

    #endregion

    #region Full Transaction Lifecycle Tests

    [Fact]
    public async Task FullTransactionLifecycle_BeginSaveCommit_ShouldWork()
    {
        // Arrange & Act - Begin
        await _unitOfWork.BeginTransactionAsync();
        _unitOfWork.HasActiveTransaction.Should().BeTrue();

        // Act - Add entity
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);
        
        // Act - Save
        var saveResult = await _unitOfWork.SaveChangesAsync();
        saveResult.Should().Be(1);

        // Act - Commit
        await _unitOfWork.CommitTransactionAsync();
        _unitOfWork.HasActiveTransaction.Should().BeFalse();

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task FullTransactionLifecycle_BeginSaveRollback_ShouldWork()
    {
        // Arrange & Act - Begin
        await _unitOfWork.BeginTransactionAsync();

        // Act - Add entity and save
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);
        await _unitOfWork.SaveChangesAsync();

        // Act - Rollback
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task MultipleTransactions_Sequential_ShouldWork()
    {
        // First transaction - commit
        await _unitOfWork.BeginTransactionAsync();
        var entity1 = new TestEntity { Name = "Entity 1", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity1);
        await _unitOfWork.CommitTransactionAsync();

        // Second transaction - commit
        await _unitOfWork.BeginTransactionAsync();
        var entity2 = new TestEntity { Name = "Entity 2", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity2);
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(2);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public async Task Dispose_WithActiveTransaction_ShouldRollback()
    {
        // Arrange
        using var context = DbContextHelper.CreateSqliteInMemoryContext();
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(context);
        await unitOfWork.BeginTransactionAsync();

        // Act
        unitOfWork.Dispose();

        // Assert
        unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public void Dispose_MultipleCalls_ShouldNotThrow()
    {
        // Arrange
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(_context);

        // Act
        var act = () =>
        {
            unitOfWork.Dispose();
            unitOfWork.Dispose();
        };

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Logging Tests

    [Fact]
    public async Task WithLogger_SaveChangesAsync_ShouldLog()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EfCoreUnitOfWork<TestDbContext>>>();
        using var context = DbContextHelper.CreateSqliteInMemoryContext();
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(context, loggerMock.Object);
        
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        context.TestEntities.Add(entity);

        // Act
        await unitOfWork.SaveChangesAsync();

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SaveChangesAsync completed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WithLogger_BeginTransactionAsync_ShouldLog()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EfCoreUnitOfWork<TestDbContext>>>();
        using var context = DbContextHelper.CreateSqliteInMemoryContext();
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(context, loggerMock.Object);

        // Act
        await unitOfWork.BeginTransactionAsync();

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Transaction started")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion
}
