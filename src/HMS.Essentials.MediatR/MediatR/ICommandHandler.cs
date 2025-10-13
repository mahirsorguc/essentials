using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Handler for commands that don't return a value.
/// Implements MediatR's IRequestHandler interface.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public interface ICommandHandler<in TCommand> : global::MediatR.IRequestHandler<TCommand, Unit>
    where TCommand : global::MediatR.IRequest<Unit>
{
}

/// <summary>
/// Handler for commands that return a value.
/// Implements MediatR's IRequestHandler interface.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response from the handler.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : global::MediatR.IRequestHandler<TCommand, TResponse>
    where TCommand : global::MediatR.IRequest<TResponse>
{
}
