using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Specifications;

/// <summary>
/// Base implementation of the specification pattern.
/// </summary>
/// <typeparam name="T">Type of the entity to query.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    /// <inheritdoc/>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc/>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <inheritdoc/>
    public List<string> IncludeStrings { get; } = new();

    /// <inheritdoc/>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc/>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <inheritdoc/>
    public int Skip { get; private set; }

    /// <inheritdoc/>
    public int Take { get; private set; }

    /// <inheritdoc/>
    public bool IsPagingEnabled { get; private set; }

    /// <inheritdoc/>
    public bool IsNoTracking { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class.
    /// </summary>
    protected Specification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class with criteria.
    /// </summary>
    /// <param name="criteria">The filter criteria.</param>
    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Sets the filter criteria.
    /// </summary>
    /// <param name="criteria">The filter criteria.</param>
    protected void SetCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds an include expression for eager loading.
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds an include string for eager loading (for nested includes).
    /// </summary>
    /// <param name="includeString">The include string.</param>
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Sets the order by expression.
    /// </summary>
    /// <param name="orderByExpression">The order by expression.</param>
    protected void SetOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Sets the order by descending expression.
    /// </summary>
    /// <param name="orderByDescendingExpression">The order by descending expression.</param>
    protected void SetOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Applies paging to the specification.
    /// </summary>
    /// <param name="skip">Number of items to skip.</param>
    /// <param name="take">Maximum number of items to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Disables change tracking for read-only queries.
    /// </summary>
    protected void ApplyNoTracking()
    {
        IsNoTracking = true;
    }
}

/// <summary>
/// A specification that always returns true (matches all entities).
/// </summary>
/// <typeparam name="T">Type of the entity.</typeparam>
public class AllSpecification<T> : Specification<T>
{
    public AllSpecification()
    {
        SetCriteria(x => true);
    }
}

/// <summary>
/// A specification based on a simple predicate expression.
/// </summary>
/// <typeparam name="T">Type of the entity.</typeparam>
public class ExpressionSpecification<T> : Specification<T>
{
    public ExpressionSpecification(Expression<Func<T, bool>> criteria) : base(criteria)
    {
    }
}
