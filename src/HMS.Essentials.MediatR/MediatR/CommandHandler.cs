using MediatR;

namespace HMS.Essentials.MediatR;

public abstract class CommandHandler<TCommand, TResponse> : BaseCqrsHandler, ICommandHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand> : BaseCqrsHandler, ICommandHandler<TCommand>
    where TCommand : IRequest<Unit>
{
    public abstract Task<Unit> Handle(TCommand request, CancellationToken cancellationToken);
}