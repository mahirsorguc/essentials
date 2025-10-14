using FluentAssertions;
using HMS.Essentials.EntityFrameworkCore.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HMS.Essentials.EntityFrameworkCore.DbContexts;

public class EfCoreDbContextTests : IDisposable
{
    private readonly TestDbContext _context;

    public EfCoreDbContextTests()
    {
        _context = DbContextHelper.CreateInMemoryContext(Guid.NewGuid().ToString());
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        // Act
        var context = new TestDbContext(options);

        // Assert
        context.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOptionsAndLogger_ShouldCreateInstance()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        var loggerMock = new Mock<ILogger<TestDbContext>>();

        // Act
        var context = new TestDbContext(options, loggerMock.Object);

        // Assert
        context.Should().NotBeNull();
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
        var result = await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
    {
        // Act
        var result = await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);

        // Act
        var result = await _context.SaveChangesAsync(cts.Token);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithAcceptAllChangesOnSuccess_ShouldWork()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);

        // Act
        var result = await _context.SaveChangesAsync(acceptAllChangesOnSuccess: true);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_MultipleEntities_ShouldReturnCorrectCount()
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
        var result = await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(3);
    }

    #endregion

    #region GetTrackedEntities Tests

    [Fact]
    public void GetTrackedEntities_WithNoEntities_ShouldReturnEmpty()
    {
        // Act
        var result = _context.GetTrackedEntities();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetTrackedEntities_WithAddedEntities_ShouldReturnTracked()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);

        // Act
        var result = _context.GetTrackedEntities();

        // Assert
        result.Should().HaveCount(1);
        result.First().State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task GetTrackedEntities_AfterSave_ShouldReturnUnchanged()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = _context.GetTrackedEntities();

        // Assert
        result.Should().HaveCount(1);
        result.First().State.Should().Be(EntityState.Unchanged);
    }

    #endregion

    #region GetTrackedEntities<TEntity> Tests

    [Fact]
    public void GetTrackedEntitiesGeneric_WithNoEntities_ShouldReturnEmpty()
    {
        // Act
        var result = _context.GetTrackedEntities<TestEntity>();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetTrackedEntitiesGeneric_WithAddedEntities_ShouldReturnTracked()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);

        // Act
        var result = _context.GetTrackedEntities<TestEntity>();

        // Assert
        result.Should().HaveCount(1);
        result.First().Entity.Name.Should().Be("Test");
    }

    [Fact]
    public void GetTrackedEntitiesGeneric_WithMixedEntities_ShouldReturnOnlySpecificType()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        var guidEntity = new TestEntityWithGuid { Name = "Guid Test", Price = 10.5m };
        _context.TestEntities.Add(testEntity);
        _context.TestEntitiesWithGuid.Add(guidEntity);

        // Act
        var result = _context.GetTrackedEntities<TestEntity>();

        // Assert
        result.Should().HaveCount(1);
        result.First().Entity.Should().BeOfType<TestEntity>();
    }

    #endregion

    #region DetachAll Tests

    [Fact]
    public void DetachAll_WithNoEntities_ShouldNotThrow()
    {
        // Act
        var act = () => _context.DetachAll();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DetachAll_WithTrackedEntities_ShouldDetachAll()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity { Name = "Entity 1", CreatedAt = DateTime.UtcNow },
            new TestEntity { Name = "Entity 2", CreatedAt = DateTime.UtcNow }
        };
        _context.TestEntities.AddRange(entities);

        // Act
        _context.DetachAll();

        // Assert
        var tracked = _context.GetTrackedEntities();
        tracked.Should().BeEmpty();
    }

    [Fact]
    public async Task DetachAll_AfterSave_ShouldDetachAll()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        _context.DetachAll();

        // Assert
        var tracked = _context.GetTrackedEntities();
        tracked.Should().BeEmpty();
    }

    #endregion

    #region GetConnectionString Tests

    [Fact]
    public void GetConnectionString_WithSqliteDatabase_ShouldReturnConnectionString()
    {
        // Arrange
        using var context = DbContextHelper.CreateSqliteInMemoryContext();

        // Act
        var result = context.GetConnectionString();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetConnectionString_WithInMemoryDatabase_ShouldThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        using var context = new TestDbContext(options);

        // Act & Assert
        // InMemory database doesn't support GetConnectionString
        // This will throw InvalidOperationException
        var act = () => context.GetConnectionString();
        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region CanConnectAsync Tests

    [Fact]
    public async Task CanConnectAsync_WithInMemoryDatabase_ShouldReturnTrue()
    {
        // Act
        var result = await _context.CanConnectAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanConnectAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _context.CanConnectAsync(cts.Token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanConnectAsync_WithSqliteDatabase_ShouldReturnTrue()
    {
        // Arrange
        using var context = DbContextHelper.CreateSqliteInMemoryContext();

        // Act
        var result = await context.CanConnectAsync();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Model Configuration Tests

    [Fact]
    public void OnModelCreating_ShouldConfigureEntities()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TestEntity));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.FindPrimaryKey().Should().NotBeNull();
        entityType.FindPrimaryKey()!.Properties.Should().HaveCount(1);
        entityType.FindPrimaryKey()!.Properties[0].Name.Should().Be("Id");
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureStringMaxLength()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TestEntity));
        var nameProperty = entityType!.FindProperty("Name");

        // Assert
        nameProperty.Should().NotBeNull();
        nameProperty!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureDecimalPrecision()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TestEntityWithGuid));
        var priceProperty = entityType!.FindProperty("Price");

        // Assert
        priceProperty.Should().NotBeNull();
        priceProperty!.GetPrecision().Should().Be(18);
        priceProperty.GetScale().Should().Be(2);
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureGuidKey()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TestEntityWithGuid));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.FindPrimaryKey().Should().NotBeNull();
        entityType.FindPrimaryKey()!.Properties[0].ClrType.Should().Be(typeof(Guid));
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureStringKey()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TestEntityWithStringKey));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.FindPrimaryKey().Should().NotBeNull();
        entityType.FindPrimaryKey()!.Properties[0].ClrType.Should().Be(typeof(string));
    }

    #endregion

    #region Logging Tests

    [Fact]
    public async Task SaveChangesAsync_WithLogger_ShouldLog()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TestDbContext>>();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new TestDbContext(options, loggerMock.Object);

        var entity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        context.TestEntities.Add(entity);

        // Act
        await context.SaveChangesAsync();

        // Assert
        // SaveChangesAsync is called twice: once by base.SaveChangesAsync and once explicitly
        // So we verify it was called at least once
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SaveChangesAsync completed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullCrudOperations_ShouldWork()
    {
        // Create
        var entity = new TestEntity
        {
            Name = "Integration Test",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Read
        var saved = await _context.TestEntities.FindAsync(entity.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Integration Test");

        // Update
        saved.Name = "Updated Name";
        _context.TestEntities.Update(saved);
        await _context.SaveChangesAsync();

        var updated = await _context.TestEntities.FindAsync(entity.Id);
        updated!.Name.Should().Be("Updated Name");

        // Delete
        _context.TestEntities.Remove(updated);
        await _context.SaveChangesAsync();

        var deleted = await _context.TestEntities.FindAsync(entity.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task MultipleEntityTypes_ShouldWork()
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test", CreatedAt = DateTime.UtcNow };
        var guidEntity = new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Guid Test", Price = 99.99m };
        var stringKeyEntity = new TestEntityWithStringKey { Id = "KEY001", Code = "CODE001" };

        // Act
        _context.TestEntities.Add(testEntity);
        _context.TestEntitiesWithGuid.Add(guidEntity);
        _context.TestEntitiesWithStringKey.Add(stringKeyEntity);
        await _context.SaveChangesAsync();

        // Assert
        var testCount = await _context.TestEntities.CountAsync();
        var guidCount = await _context.TestEntitiesWithGuid.CountAsync();
        var stringKeyCount = await _context.TestEntitiesWithStringKey.CountAsync();

        testCount.Should().Be(1);
        guidCount.Should().Be(1);
        stringKeyCount.Should().Be(1);
    }

    #endregion
}
