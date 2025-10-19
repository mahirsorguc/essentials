using MediatR;

namespace HMS.Essentials.MediatR;

public abstract class QueryHandler<TQuery, TResponse> : BaseCqrsHandler, IQueryHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);
}