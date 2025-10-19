using HMS.Essentials.Modularity.DependencyInjection;
using HMS.Essentials.SequentialGuid;
using MediatR;

namespace HMS.Essentials.MediatR;

public abstract class CommandHandler
{
    [InjectProperty] 
    public required ISequentialGuidGenerator SequentialGuidGenerator { get; set; }
}

public abstract class CommandHandler<TCommand> : CommandHandler, ICommandHandler<TCommand>
    where TCommand : IRequest<Unit>
{
    public abstract Task<Unit> Handle(TCommand request, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand, TResponse> : CommandHandler, ICommandHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
}