using Moq;
using Shouldly;

namespace HMS.Essentials.UnitOfWork.Tests;

/// <summary>
/// Tests for IUnitOfWork interface contract.
/// </summary>
public class IUnitOfWorkTests
{
    [Fact]
    public void IUnitOfWork_ShouldBeInterface()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Assert
        type.IsInterface.ShouldBeTrue();
    }

    [Fact]
    public void IUnitOfWork_ShouldImplementIDisposable()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Assert
        typeof(IDisposable).IsAssignableFrom(type).ShouldBeTrue();
    }

    [Fact]
    public void IUnitOfWork_ShouldHaveSaveChangesAsyncMethod()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Act
        var method = type.GetMethod(nameof(IUnitOfWork.SaveChangesAsync));

        // Assert
        method.ShouldNotBeNull();
        method.ReturnType.ShouldBe(typeof(Task<int>));
    }

    [Fact]
    public void IUnitOfWork_ShouldHaveBeginTransactionAsyncMethod()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Act
        var method = type.GetMethod(nameof(IUnitOfWork.BeginTransactionAsync));

        // Assert
        method.ShouldNotBeNull();
        method.ReturnType.ShouldBe(typeof(Task));
    }

    [Fact]
    public void IUnitOfWork_ShouldHaveCommitTransactionAsyncMethod()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Act
        var method = type.GetMethod(nameof(IUnitOfWork.CommitTransactionAsync));

        // Assert
        method.ShouldNotBeNull();
        method.ReturnType.ShouldBe(typeof(Task));
    }

    [Fact]
    public void IUnitOfWork_ShouldHaveRollbackTransactionAsyncMethod()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Act
        var method = type.GetMethod(nameof(IUnitOfWork.RollbackTransactionAsync));

        // Assert
        method.ShouldNotBeNull();
        method.ReturnType.ShouldBe(typeof(Task));
    }

    [Fact]
    public void IUnitOfWork_ShouldHaveHasActiveTransactionProperty()
    {
        // Arrange
        var type = typeof(IUnitOfWork);

        // Act
        var property = type.GetProperty(nameof(IUnitOfWork.HasActiveTransaction));

        // Assert
        property.ShouldNotBeNull();
        property.PropertyType.ShouldBe(typeof(bool));
        property.CanRead.ShouldBeTrue();
    }

    [Fact]
    public async Task MockedUnitOfWork_SaveChangesAsync_ShouldReturnExpectedValue()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await mockUnitOfWork.Object.SaveChangesAsync();

        // Assert
        result.ShouldBe(5);
    }

    [Fact]
    public async Task MockedUnitOfWork_BeginTransactionAsync_ShouldComplete()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Should.NotThrowAsync(async () => 
            await mockUnitOfWork.Object.BeginTransactionAsync());
    }

    [Fact]
    public async Task MockedUnitOfWork_CommitTransactionAsync_ShouldComplete()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Should.NotThrowAsync(async () => 
            await mockUnitOfWork.Object.CommitTransactionAsync());
    }

    [Fact]
    public async Task MockedUnitOfWork_RollbackTransactionAsync_ShouldComplete()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Should.NotThrowAsync(async () => 
            await mockUnitOfWork.Object.RollbackTransactionAsync());
    }

    [Fact]
    public void MockedUnitOfWork_HasActiveTransaction_ShouldReturnExpectedValue()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(x => x.HasActiveTransaction).Returns(true);

        // Act
        var result = mockUnitOfWork.Object.HasActiveTransaction;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void MockedUnitOfWork_Dispose_ShouldNotThrow()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        // Act & Assert
        Should.NotThrow(() => mockUnitOfWork.Object.Dispose());
    }

    [Fact]
    public async Task MockedUnitOfWork_SaveChangesAsync_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var cts = new CancellationTokenSource();
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(cts.Token))
            .ReturnsAsync(3);

        // Act
        var result = await mockUnitOfWork.Object.SaveChangesAsync(cts.Token);

        // Assert
        result.ShouldBe(3);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task MockedUnitOfWork_BeginTransactionAsync_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var cts = new CancellationTokenSource();
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(cts.Token))
            .Returns(Task.CompletedTask);

        // Act
        await mockUnitOfWork.Object.BeginTransactionAsync(cts.Token);

        // Assert
        mockUnitOfWork.Verify(x => x.BeginTransactionAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task MockedUnitOfWork_CommitTransactionAsync_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var cts = new CancellationTokenSource();
        mockUnitOfWork.Setup(x => x.CommitTransactionAsync(cts.Token))
            .Returns(Task.CompletedTask);

        // Act
        await mockUnitOfWork.Object.CommitTransactionAsync(cts.Token);

        // Assert
        mockUnitOfWork.Verify(x => x.CommitTransactionAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task MockedUnitOfWork_RollbackTransactionAsync_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var cts = new CancellationTokenSource();
        mockUnitOfWork.Setup(x => x.RollbackTransactionAsync(cts.Token))
            .Returns(Task.CompletedTask);

        // Act
        await mockUnitOfWork.Object.RollbackTransactionAsync(cts.Token);

        // Assert
        mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task MockedUnitOfWork_TransactionWorkflow_ShouldExecuteCorrectly()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        mockUnitOfWork.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(x => x.HasActiveTransaction).Returns(true);

        // Act
        await mockUnitOfWork.Object.BeginTransactionAsync();
        var savedCount = await mockUnitOfWork.Object.SaveChangesAsync();
        await mockUnitOfWork.Object.CommitTransactionAsync();

        // Assert
        savedCount.ShouldBe(2);
        mockUnitOfWork.Object.HasActiveTransaction.ShouldBeTrue();
        mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
