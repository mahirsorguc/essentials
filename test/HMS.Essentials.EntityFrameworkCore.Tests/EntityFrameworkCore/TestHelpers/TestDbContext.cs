using HMS.Essentials.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.EntityFrameworkCore.TestHelpers;

/// <summary>
/// Test DbContext for unit tests.
/// </summary>
public class TestDbContext : EfCoreDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public TestDbContext(DbContextOptions<TestDbContext> options, ILogger<TestDbContext> logger) 
        : base(options, logger)
    {
    }

    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    public DbSet<TestEntityWithGuid> TestEntitiesWithGuid => Set<TestEntityWithGuid>();
    public DbSet<TestEntityWithStringKey> TestEntitiesWithStringKey => Set<TestEntityWithStringKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<TestEntityWithGuid>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<TestEntityWithStringKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
        });
    }
}
