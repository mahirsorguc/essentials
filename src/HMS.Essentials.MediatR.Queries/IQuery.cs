using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Marker interface for queries.
/// Queries represent read-only operations that return data.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
