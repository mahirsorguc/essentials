using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Marker interface for commands that don't return a value.
/// Commands represent actions that modify state.
/// </summary>
public interface ICommand : IRequest<Unit>
{
}

/// <summary>
/// Marker interface for commands that return a value.
/// Commands represent actions that modify state.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
