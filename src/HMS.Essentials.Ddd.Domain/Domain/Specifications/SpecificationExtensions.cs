namespace HMS.Essentials.Domain.Specifications;

/// <summary>
/// Extension methods for combining specifications using logical operators.
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// Combines two specifications using AND logic.
    /// </summary>
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    /// <summary>
    /// Combines two specifications using OR logic.
    /// </summary>
    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    /// <summary>
    /// Negates a specification.
    /// </summary>
    public static ISpecification<T> Not<T>(this ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }
}
