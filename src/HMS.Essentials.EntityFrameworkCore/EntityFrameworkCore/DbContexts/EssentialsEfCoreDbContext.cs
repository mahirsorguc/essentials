using HMS.Essentials.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.EntityFrameworkCore.DbContexts;

/// <summary>
/// Base class for Entity Framework Core DbContext with enhanced functionality.
/// Provides support for domain events, soft delete, auditing, and advanced tracking.
/// </summary>
public abstract class EssentialsEfCoreDbContext : DbContext
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EssentialsEfCoreDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    protected EssentialsEfCoreDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EssentialsEfCoreDbContext"/> class with logging support.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    /// <param name="logger">The logger instance.</param>
    protected EssentialsEfCoreDbContext(DbContextOptions options, ILogger logger) : base(options)
    {
        _logger = logger;
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            
            _logger?.LogDebug("SaveChangesAsync completed successfully. {Count} entities affected.", result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while saving changes to the database.");
            throw;
        }
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">
    /// Indicates whether Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges
    /// is called after the changes have been sent successfully to the database.
    /// </param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            
            _logger?.LogDebug("SaveChangesAsync completed successfully. {Count} entities affected.", result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while saving changes to the database.");
            throw;
        }
    }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        _logger?.LogDebug("Configuring Entity Framework Core model.");
        
        ConfigureBaseEntities(modelBuilder);
        ConfigureConventions(modelBuilder);
    }

    /// <summary>
    /// Configures base entity properties and conventions.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected virtual void ConfigureBaseEntities(ModelBuilder modelBuilder)
    {
        // Configure entities with IEntity<TKey> interface
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity implements IEntity<TKey>
            var entityInterface = entityType.ClrType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                                    i.GetGenericTypeDefinition() == typeof(IEntity<>));

            if (entityInterface != null)
            {
                ConfigureEntityType(modelBuilder, entityType, entityInterface);
            }
        }
    }

    /// <summary>
    /// Configures a specific entity type.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="entityType">The entity type metadata.</param>
    /// <param name="entityInterface">The IEntity interface type.</param>
    protected virtual void ConfigureEntityType(
        ModelBuilder modelBuilder,
        Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType,
        Type entityInterface)
    {
        // Configure primary key if not already configured
        if (entityType.FindPrimaryKey() == null)
        {
            var keyType = entityInterface.GetGenericArguments()[0];
            var idProperty = entityType.FindProperty("Id");
            
            if (idProperty != null)
            {
                entityType.SetPrimaryKey(idProperty);
            }
        }
    }

    /// <summary>
    /// Configures model-wide conventions.
    /// Override this method to add custom conventions.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected virtual void ConfigureConventions(ModelBuilder modelBuilder)
    {
        // Default implementations can be added here
        // e.g., string length conventions, decimal precision, etc.
    }

    /// <summary>
    /// Gets all tracked entities in the context.
    /// </summary>
    /// <returns>Collection of entity entries.</returns>
    public IEnumerable<EntityEntry> GetTrackedEntities()
    {
        return ChangeTracker.Entries();
    }

    /// <summary>
    /// Gets tracked entities of a specific type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>Collection of entity entries.</returns>
    public IEnumerable<EntityEntry<TEntity>> GetTrackedEntities<TEntity>() where TEntity : class
    {
        return ChangeTracker.Entries<TEntity>();
    }

    /// <summary>
    /// Detaches all tracked entities from the context.
    /// </summary>
    public void DetachAll()
    {
        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            entry.State = EntityState.Detached;
        }
    }

    /// <summary>
    /// Gets the connection string used by this context.
    /// </summary>
    /// <returns>The connection string or null if not applicable.</returns>
    public virtual string? GetConnectionString()
    {
        return Database.GetConnectionString();
    }

    /// <summary>
    /// Checks if the database connection can be established.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the database can be connected, false otherwise.</returns>
    public virtual async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to connect to the database.");
            return false;
        }
    }
}
