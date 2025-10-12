using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Specifications;

/// <summary>
/// Represents the AND combination of two specifications.
/// </summary>
internal class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;

        if (_left.Criteria != null && _right.Criteria != null)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftExpression = new ParameterReplacer(parameter).Visit(_left.Criteria.Body)!;
            var rightExpression = new ParameterReplacer(parameter).Visit(_right.Criteria.Body)!;
            var combined = Expression.AndAlso(leftExpression, rightExpression);
            SetCriteria(Expression.Lambda<Func<T, bool>>(combined, parameter));
        }
        else if (_left.Criteria != null)
        {
            SetCriteria(_left.Criteria);
        }
        else if (_right.Criteria != null)
        {
            SetCriteria(_right.Criteria);
        }
    }
}

/// <summary>
/// Represents the OR combination of two specifications.
/// </summary>
internal class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;

        if (_left.Criteria != null && _right.Criteria != null)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftExpression = new ParameterReplacer(parameter).Visit(_left.Criteria.Body)!;
            var rightExpression = new ParameterReplacer(parameter).Visit(_right.Criteria.Body)!;
            var combined = Expression.OrElse(leftExpression, rightExpression);
            SetCriteria(Expression.Lambda<Func<T, bool>>(combined, parameter));
        }
        else if (_left.Criteria != null)
        {
            SetCriteria(_left.Criteria);
        }
        else if (_right.Criteria != null)
        {
            SetCriteria(_right.Criteria);
        }
    }
}

/// <summary>
/// Represents the NOT (negation) of a specification.
/// </summary>
internal class NotSpecification<T> : Specification<T>
{
    public NotSpecification(ISpecification<T> specification)
    {
        if (specification.Criteria != null)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = new ParameterReplacer(parameter).Visit(specification.Criteria.Body)!;
            var notExpression = Expression.Not(body);
            SetCriteria(Expression.Lambda<Func<T, bool>>(notExpression, parameter));
        }
    }
}

/// <summary>
/// Helper class to replace parameters in expressions when combining specifications.
/// </summary>
internal class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    public ParameterReplacer(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameter;
    }
}
