using HMS.Essentials.Domain.Entities;
using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Repositories;

/// <summary>
/// Abstract base implementation of a repository with full CRUD operations.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public abstract class RepositoryBase<TEntity, TKey> : ReadOnlyRepositoryBase<TEntity, TKey>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <inheritdoc/>
    public abstract Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await InsertAsync(entity, false, cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public abstract Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await UpdateAsync(entity, false, cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public abstract Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, autoSave, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, false, cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteManyAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entities = await GetListAsync(predicate, cancellationToken);
        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }

    /// <summary>
    /// Saves all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of affected records.</returns>
    protected abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
