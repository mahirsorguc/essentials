using HMS.Essentials.Domain.Entities;
using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Repositories;

/// <summary>
/// Base interface for all repositories with full CRUD operations.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The inserted entity.</returns>
    Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts multiple entities.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InsertManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(
        TKey id,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple entities.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entities matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="autoSave">If true, saves changes automatically.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool autoSave = false,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for entities with integer keys.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, int>, IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<int>
{
}
