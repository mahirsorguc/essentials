using Microsoft.EntityFrameworkCore;

namespace HMS.Essentials.EntityFrameworkCore.TestHelpers;

/// <summary>
/// Helper class to create in-memory database instances for testing.
/// </summary>
public static class DbContextHelper
{
    /// <summary>
    /// Creates a new TestDbContext with in-memory database.
    /// </summary>
    public static TestDbContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new TestDbContext(options);
        return context;
    }

    /// <summary>
    /// Creates a new TestDbContext with SQLite in-memory database.
    /// </summary>
    public static TestDbContext CreateSqliteInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new TestDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    /// <summary>
    /// Seeds test data into the database.
    /// </summary>
    public static void SeedTestData(TestDbContext context)
    {
        var entities = new[]
        {
            new TestEntity { Id = 1, Name = "Entity 1", Description = "Description 1", CreatedAt = DateTime.UtcNow, IsActive = true },
            new TestEntity { Id = 2, Name = "Entity 2", Description = "Description 2", CreatedAt = DateTime.UtcNow, IsActive = true },
            new TestEntity { Id = 3, Name = "Entity 3", Description = "Description 3", CreatedAt = DateTime.UtcNow, IsActive = false },
            new TestEntity { Id = 4, Name = "Entity 4", Description = null, CreatedAt = DateTime.UtcNow, IsActive = true },
            new TestEntity { Id = 5, Name = "Entity 5", Description = "Description 5", CreatedAt = DateTime.UtcNow, IsActive = false }
        };

        context.TestEntities.AddRange(entities);
        context.SaveChanges();
    }
}
