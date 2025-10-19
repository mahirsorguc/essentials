using HMS.Essentials.SequentialGuid;
using MediatR;

namespace HMS.Essentials.MediatR;

public abstract class CommandHandler
{
    protected readonly ISequentialGuidGenerator SequentialGuidGenerator;

    protected CommandHandler(ISequentialGuidGenerator sequentialGuidGenerator)
    {
        SequentialGuidGenerator = sequentialGuidGenerator;
    }
}

public abstract class CommandHandler<TCommand> : CommandHandler, ICommandHandler<TCommand>
    where TCommand : IRequest<Unit>
{
    protected CommandHandler(ISequentialGuidGenerator sequentialGuidGenerator) : base(sequentialGuidGenerator)
    {
    }

    public abstract Task<Unit> Handle(TCommand request, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand, TResponse> : CommandHandler, ICommandHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    protected CommandHandler(ISequentialGuidGenerator sequentialGuidGenerator) : base(sequentialGuidGenerator)
    {
    }

    public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
}