using FluentAssertions;
using HMS.Essentials.EntityFrameworkCore.TestHelpers;

namespace HMS.Essentials.EntityFrameworkCore.Repositories;

public class EfCoreRepositoryWithDifferentKeysTests : IDisposable
{
    private readonly TestDbContext _context;

    public EfCoreRepositoryWithDifferentKeysTests()
    {
        _context = DbContextHelper.CreateInMemoryContext(Guid.NewGuid().ToString());
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region Repository with Guid Key Tests

    [Fact]
    public async Task GuidKeyRepository_GetByIdAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var id = Guid.NewGuid();
        var entity = new TestEntityWithGuid
        {
            Id = id,
            Name = "Guid Entity",
            Price = 100.50m
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Guid Entity");
    }

    [Fact]
    public async Task GuidKeyRepository_InsertAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var entity = new TestEntityWithGuid
        {
            Id = Guid.NewGuid(),
            Name = "New Guid Entity",
            Price = 50.25m
        };

        // Act
        var result = await repository.InsertAsync(entity, autoSave: true);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GuidKeyRepository_UpdateAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var id = Guid.NewGuid();
        var entity = new TestEntityWithGuid
        {
            Id = id,
            Name = "Original Name",
            Price = 100m
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        entity.Name = "Updated Name";
        entity.Price = 200m;
        _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        await repository.UpdateAsync(entity, autoSave: true);

        // Assert
        var updated = await repository.GetByIdAsync(id);
        updated!.Name.Should().Be("Updated Name");
        updated.Price.Should().Be(200m);
    }

    [Fact]
    public async Task GuidKeyRepository_DeleteAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var id = Guid.NewGuid();
        var entity = new TestEntityWithGuid
        {
            Id = id,
            Name = "To Delete",
            Price = 10m
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        await repository.DeleteAsync(id, autoSave: true);

        // Assert
        var deleted = await repository.GetByIdAsync(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GuidKeyRepository_GetListAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var entities = new[]
        {
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Entity 1", Price = 10m },
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Entity 2", Price = 20m },
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Entity 3", Price = 30m }
        };
        await repository.InsertManyAsync(entities, autoSave: true);

        // Act
        var result = await repository.GetListAsync(e => e.Price > 15m);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Price > 15m);
    }

    #endregion

    #region Repository with String Key Tests

    [Fact]
    public async Task StringKeyRepository_GetByIdAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);
        var entity = new TestEntityWithStringKey
        {
            Id = "KEY001",
            Code = "CODE001"
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        var result = await repository.GetByIdAsync("KEY001");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("KEY001");
        result.Code.Should().Be("CODE001");
    }

    [Fact]
    public async Task StringKeyRepository_InsertAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);
        var entity = new TestEntityWithStringKey
        {
            Id = "KEY002",
            Code = "CODE002"
        };

        // Act
        var result = await repository.InsertAsync(entity, autoSave: true);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("KEY002");
    }

    [Fact]
    public async Task StringKeyRepository_UpdateAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);
        var entity = new TestEntityWithStringKey
        {
            Id = "KEY003",
            Code = "ORIGINAL"
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        entity.Code = "UPDATED";
        _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        await repository.UpdateAsync(entity, autoSave: true);

        // Assert
        var updated = await repository.GetByIdAsync("KEY003");
        updated!.Code.Should().Be("UPDATED");
    }

    [Fact]
    public async Task StringKeyRepository_DeleteAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);
        var entity = new TestEntityWithStringKey
        {
            Id = "KEY004",
            Code = "CODE004"
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        await repository.DeleteAsync("KEY004", autoSave: true);

        // Assert
        var deleted = await repository.GetByIdAsync("KEY004");
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task StringKeyRepository_CountAsync_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);
        var entities = new[]
        {
            new TestEntityWithStringKey { Id = "KEY005", Code = "CODE005" },
            new TestEntityWithStringKey { Id = "KEY006", Code = "CODE006" },
            new TestEntityWithStringKey { Id = "KEY007", Code = "CODE007" }
        };
        await repository.InsertManyAsync(entities, autoSave: true);

        // Act
        var count = await repository.CountAsync();

        // Assert
        count.Should().Be(3);
    }

    #endregion

    #region Edge Cases and Special Scenarios

    [Fact]
    public async Task Repository_WithEmptyGuid_ShouldHandleCorrectly()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);

        // Act
        var result = await repository.GetByIdAsync(Guid.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Repository_WithNullStringKey_ShouldHandleCorrectly()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);

        // Act
        var result = await repository.GetByIdAsync(null!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Repository_WithEmptyStringKey_ShouldHandleCorrectly()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);

        // Act
        var result = await repository.GetByIdAsync(string.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Repository_InsertManyWithDifferentKeyTypes_ShouldWork()
    {
        // Arrange
        var guidRepository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var stringRepository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);

        var guidEntities = new[]
        {
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Guid 1", Price = 10m },
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Guid 2", Price = 20m }
        };

        var stringEntities = new[]
        {
            new TestEntityWithStringKey { Id = "STR001", Code = "CODE001" },
            new TestEntityWithStringKey { Id = "STR002", Code = "CODE002" }
        };

        // Act
        await guidRepository.InsertManyAsync(guidEntities, autoSave: true);
        await stringRepository.InsertManyAsync(stringEntities, autoSave: true);

        // Assert
        var guidCount = await guidRepository.CountAsync();
        var stringCount = await stringRepository.CountAsync();

        guidCount.Should().Be(2);
        stringCount.Should().Be(2);
    }

    [Fact]
    public async Task Repository_GetPagedListWithDifferentKeyTypes_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var entities = Enumerable.Range(1, 10)
            .Select(i => new TestEntityWithGuid
            {
                Id = Guid.NewGuid(),
                Name = $"Entity {i}",
                Price = i * 10m
            })
            .ToArray();
        await repository.InsertManyAsync(entities, autoSave: true);

        // Act
        var page1 = await repository.GetPagedListAsync(skipCount: 0, maxResultCount: 3);
        var page2 = await repository.GetPagedListAsync(skipCount: 3, maxResultCount: 3);

        // Assert
        page1.Should().HaveCount(3);
        page2.Should().HaveCount(3);
    }

    [Fact]
    public async Task Repository_AnyAsyncWithDifferentKeyTypes_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithStringKey, string>(_context);
        var entity = new TestEntityWithStringKey
        {
            Id = "UNIQUE_KEY",
            Code = "UNIQUE_CODE"
        };
        await repository.InsertAsync(entity, autoSave: true);

        // Act
        var exists = await repository.AnyAsync(e => e.Code == "UNIQUE_CODE");
        var notExists = await repository.AnyAsync(e => e.Code == "NON_EXISTENT");

        // Assert
        exists.Should().BeTrue();
        notExists.Should().BeFalse();
    }

    [Fact]
    public async Task Repository_GetQueryableWithDifferentKeyTypes_ShouldWork()
    {
        // Arrange
        var repository = new EfCoreRepository<TestDbContext, TestEntityWithGuid, Guid>(_context);
        var entities = new[]
        {
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Entity A", Price = 100m },
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Entity B", Price = 200m },
            new TestEntityWithGuid { Id = Guid.NewGuid(), Name = "Entity C", Price = 300m }
        };
        await repository.InsertManyAsync(entities, autoSave: true);

        // Act
        var queryable = repository.GetQueryable();
        var filtered = queryable.Where(e => e.Price >= 200m);
        var result = filtered.ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Price >= 200m);
    }

    #endregion
}
