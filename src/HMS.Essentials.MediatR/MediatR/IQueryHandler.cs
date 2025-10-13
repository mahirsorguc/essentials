namespace HMS.Essentials.MediatR;

/// <summary>
/// Handler for queries.
/// Implements MediatR's IRequestHandler interface.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of response from the handler.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : global::MediatR.IRequestHandler<TQuery, TResponse>
    where TQuery : global::MediatR.IRequest<TResponse>
{
}
