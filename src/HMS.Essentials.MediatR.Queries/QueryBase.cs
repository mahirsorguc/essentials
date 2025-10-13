using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Base class for queries.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract record QueryBase<TResponse> : IQuery<TResponse>, IRequest<TResponse>
{
    /// <summary>
    /// Gets the timestamp when the query was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}
