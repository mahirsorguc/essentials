using HMS.Essentials.Domain.Entities;
using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Repositories;

/// <summary>
/// Base interface for all read-only repositories.
/// Provides query operations without modification capabilities.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IReadOnlyRepository<TEntity, TKey> 
    where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity or null if not found.</returns>
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all entities.</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching entities.</returns>
    Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queryable collection of entities.
    /// </summary>
    /// <returns>Queryable collection.</returns>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// Checks if any entity matches the predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any entity matches, false otherwise.</returns>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total count of entities.</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Count of matching entities.</returns>
    Task<long> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of entities.
    /// </summary>
    /// <param name="skipCount">Number of entities to skip.</param>
    /// <param name="maxResultCount">Maximum number of entities to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of entities.</returns>
    Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of entities matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="skipCount">Number of entities to skip.</param>
    /// <param name="maxResultCount">Maximum number of entities to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of matching entities.</returns>
    Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Read-only repository for entities with integer keys.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
public interface IReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity, int>
    where TEntity : class, IEntity<int>
{
}
