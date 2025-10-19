using HMS.Essentials.SequentialGuid;
using MediatR;

namespace HMS.Essentials.MediatR;

public abstract class QueryHandler
{
    protected readonly ISequentialGuidGenerator SequentialGuidGenerator;

    protected QueryHandler(ISequentialGuidGenerator sequentialGuidGenerator)
    {
        SequentialGuidGenerator = sequentialGuidGenerator;
    }
}

public abstract class QueryHandler<TQuery, TResponse> : QueryHandler, IQueryHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>
{
    protected QueryHandler(ISequentialGuidGenerator sequentialGuidGenerator) : base(sequentialGuidGenerator)
    {
    }

    public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);
}