using FluentAssertions;
using HMS.Essentials.EntityFrameworkCore.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HMS.Essentials.EntityFrameworkCore.Repositories;

public class EfCoreRepositoryTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly EfCoreRepository<TestDbContext, TestEntity, int> _repository;

    public EfCoreRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext(Guid.NewGuid().ToString());
        _repository = new EfCoreRepository<TestDbContext, TestEntity, int>(_context);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDbContext_ShouldCreateInstance()
    {
        // Arrange & Act
        var repository = new EfCoreRepository<TestDbContext, TestEntity, int>(_context);

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullDbContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new EfCoreRepository<TestDbContext, TestEntity, int>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    #endregion

    #region Query Operations Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Entity 1");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithCancellationToken_ShouldSupportCancellation()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _repository.GetByIdAsync(1, cts.Token);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_WithValidPredicate_ShouldReturnFirstMatch()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetAsync(e => e.Name == "Entity 2");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
        result.Name.Should().Be("Entity 2");
    }

    [Fact]
    public async Task GetAsync_WithNoMatch_ShouldReturnNull()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetAsync(e => e.Name == "NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetListAsync_WithPredicate_ShouldReturnMatchingEntities()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetListAsync(e => e.IsActive);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().OnlyContain(e => e.IsActive);
    }

    [Fact]
    public async Task GetListAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetListAsync(e => e.Name == "NonExistent");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetQueryable_ShouldReturnQueryable()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var queryable = _repository.GetQueryable();

        // Assert
        queryable.Should().NotBeNull();
        queryable.Should().BeAssignableTo<IQueryable<TestEntity>>();
    }

    [Fact]
    public async Task GetQueryable_ShouldAllowLinqOperations()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetQueryable()
            .Where(e => e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();

        // Assert
        result.Should().HaveCount(3);
        result.First().Name.Should().Be("Entity 1");
    }

    [Fact]
    public async Task AnyAsync_WithMatchingPredicate_ShouldReturnTrue()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.AnyAsync(e => e.Name == "Entity 1");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_WithNoMatch_ShouldReturnFalse()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.AnyAsync(e => e.Name == "NonExistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task CountAsync_WithEmptyDatabase_ShouldReturnZero()
    {
        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnMatchingCount()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.CountAsync(e => e.IsActive);

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetPagedListAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetPagedListAsync(skipCount: 1, maxResultCount: 2);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(2);
        result[1].Id.Should().Be(3);
    }

    [Fact]
    public async Task GetPagedListAsync_WithPredicate_ShouldReturnCorrectPage()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var result = await _repository.GetPagedListAsync(
            predicate: e => e.IsActive,
            skipCount: 0,
            maxResultCount: 2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.IsActive);
    }

    #endregion

    #region Insert Operations Tests

    [Fact]
    public async Task InsertAsync_WithValidEntity_ShouldAddEntity()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "New Entity",
            Description = "New Description",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.InsertAsync(entity, autoSave: true);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(0);
        
        var saved = await _context.TestEntities.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Entity");
    }

    [Fact]
    public async Task InsertAsync_WithAutoSaveFalse_ShouldNotPersist()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "New Entity",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.InsertAsync(entity, autoSave: false);

        // Assert
        result.Should().NotBeNull();
        _context.Entry(result).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task InsertManyAsync_WithValidEntities_ShouldAddAllEntities()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity { Name = "Entity A", CreatedAt = DateTime.UtcNow },
            new TestEntity { Name = "Entity B", CreatedAt = DateTime.UtcNow },
            new TestEntity { Name = "Entity C", CreatedAt = DateTime.UtcNow }
        };

        // Act
        await _repository.InsertManyAsync(entities, autoSave: true);

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task InsertManyAsync_WithAutoSaveFalse_ShouldNotPersist()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity { Name = "Entity A", CreatedAt = DateTime.UtcNow },
            new TestEntity { Name = "Entity B", CreatedAt = DateTime.UtcNow }
        };

        // Act
        await _repository.InsertManyAsync(entities, autoSave: false);

        // Assert
        foreach (var entity in entities)
        {
            _context.Entry(entity).State.Should().Be(EntityState.Added);
        }
    }

    #endregion

    #region Update Operations Tests

    [Fact]
    public async Task UpdateAsync_WithExistingEntity_ShouldUpdateEntity()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        var entity = await _context.TestEntities.FindAsync(1);
        entity!.Name = "Updated Name";
        _context.Entry(entity).State = EntityState.Detached;

        // Act
        var result = await _repository.UpdateAsync(entity, autoSave: true);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        
        var updated = await _context.TestEntities.FindAsync(1);
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WithAutoSaveFalse_ShouldNotPersist()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        var entity = await _context.TestEntities.FindAsync(1);
        entity!.Name = "Updated Name";
        _context.Entry(entity).State = EntityState.Detached;

        // Act
        var result = await _repository.UpdateAsync(entity, autoSave: false);

        // Assert
        _context.Entry(result).State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public async Task UpdateManyAsync_WithValidEntities_ShouldUpdateAllEntities()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        var entities = await _context.TestEntities.Take(2).ToListAsync();
        foreach (var entity in entities)
        {
            entity.Name += " - Updated";
            _context.Entry(entity).State = EntityState.Detached;
        }

        // Act
        await _repository.UpdateManyAsync(entities, autoSave: true);

        // Assert
        var updated = await _context.TestEntities.Take(2).ToListAsync();
        updated.Should().AllSatisfy(e => e.Name.Should().Contain("Updated"));
    }

    #endregion

    #region Delete Operations Tests

    [Fact]
    public async Task DeleteAsync_WithEntity_ShouldRemoveEntity()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        var entity = await _context.TestEntities.FindAsync(1);

        // Act
        await _repository.DeleteAsync(entity!, autoSave: true);

        // Assert
        var deleted = await _context.TestEntities.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithId_ShouldRemoveEntity()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        await _repository.DeleteAsync(1, autoSave: true);

        // Assert
        var deleted = await _context.TestEntities.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldNotThrow()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        var act = async () => await _repository.DeleteAsync(999, autoSave: true);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteManyAsync_WithEntities_ShouldRemoveAllEntities()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        var entities = await _context.TestEntities.Take(3).ToListAsync();

        // Act
        await _repository.DeleteManyAsync(entities, autoSave: true);

        // Assert
        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task DeleteManyAsync_WithPredicate_ShouldRemoveMatchingEntities()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);

        // Act
        await _repository.DeleteManyAsync(e => !e.IsActive, autoSave: true);

        // Assert
        var remaining = await _context.TestEntities.ToListAsync();
        remaining.Should().HaveCount(3);
        remaining.Should().OnlyContain(e => e.IsActive);
    }

    #endregion

    #region Repository with Default Key Type Tests

    [Fact]
    public void DefaultKeyTypeRepository_ShouldWorkWithIntKey()
    {
        // Arrange & Act
        var repository = new EfCoreRepository<TestDbContext, TestEntity>(_context);

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<EfCoreRepository<TestDbContext, TestEntity, int>>();
    }

    [Fact]
    public async Task DefaultKeyTypeRepository_GetByIdAsync_ShouldWork()
    {
        // Arrange
        DbContextHelper.SeedTestData(_context);
        var repository = new EfCoreRepository<TestDbContext, TestEntity>(_context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    #endregion
}
