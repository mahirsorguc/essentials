using HMS.Essentials.Domain.Entities;
using HMS.Essentials.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HMS.Essentials.EntityFrameworkCore.Repositories;

/// <summary>
/// Entity Framework Core implementation of the repository pattern.
/// Provides full CRUD operations for entities.
/// </summary>
/// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public class EfCoreRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
{
    protected readonly TDbContext DbContext;
    protected virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreRepository{TDbContext, TEntity, TKey}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public EfCoreRepository(TDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    #region Query Operations

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id! }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    /// <inheritdoc/>
    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(predicate)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Insert Operations

    /// <inheritdoc/>
    public virtual async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var savedEntity = (await DbSet.AddAsync(entity, cancellationToken)).Entity;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return savedEntity;
    }

    /// <inheritdoc/>
    public virtual async Task InsertManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();

        await DbSet.AddRangeAsync(entityList, cancellationToken);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Update Operations

    /// <inheritdoc/>
    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbContext.Attach(entity);
        
        var updatedEntity = DbContext.Update(entity).Entity;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return updatedEntity;
    }

    /// <inheritdoc/>
    public virtual async Task UpdateManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();

        DbSet.UpdateRange(entityList);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Delete Operations

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(
        TKey id,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        
        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();

        DbSet.RemoveRange(entityList);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entities = await GetListAsync(predicate, cancellationToken);

        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }

    #endregion
}

/// <summary>
/// Entity Framework Core repository for entities with integer keys.
/// </summary>
/// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
public class EfCoreRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity, int>, IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreRepository{TDbContext, TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public EfCoreRepository(TDbContext dbContext) : base(dbContext)
    {
    }
}
