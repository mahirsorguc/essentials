using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Specifications;

/// <summary>
/// Represents a specification pattern for building complex queries.
/// </summary>
/// <typeparam name="T">Type of the entity to query.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the filter criteria expression.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the list of include expressions for eager loading.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the list of include strings for eager loading (used for nested includes).
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the order by expression.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the order by descending expression.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Gets the maximum number of items to take.
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Gets a value indicating whether paging is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether tracking is disabled (for read-only queries).
    /// </summary>
    bool IsNoTracking { get; }
}
