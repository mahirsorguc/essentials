using HMS.Essentials.EntityFrameworkCore.DbContexts;
using HMS.MainApp.Samples;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.MainApp.EntityFrameworkCore;

public class MainAppDbContext : EssentialsEfCoreDbContext
{
    public MainAppDbContext(DbContextOptions options) : base(options)
    {
    }

    public MainAppDbContext(DbContextOptions options, ILogger logger) : base(options, logger)
    {
    }

    public DbSet<Sample> Samples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sample>(builder =>
        {
            const string tableNamePrefix = "App";
            builder.ToTable($"{tableNamePrefix}Samples");
            builder.Property(prop => prop.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(prop => prop.Description)
                .HasColumnName("Description");
        });
    }
}