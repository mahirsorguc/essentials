using HMS.Essentials.Domain.Entities;
using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Repositories;

/// <summary>
/// Abstract base implementation of a read-only repository.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public abstract class ReadOnlyRepositoryBase<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <inheritdoc/>
    public abstract Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract IQueryable<TEntity> GetQueryable();

    /// <inheritdoc/>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return GetQueryable().Any(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return GetQueryable().LongCount();
    }

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return GetQueryable().LongCount(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, CancellationToken cancellationToken = default)
    {
        return GetQueryable()
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToList();
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return GetQueryable()
            .Where(predicate)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToList();
    }
}
