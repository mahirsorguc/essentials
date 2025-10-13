using MediatR;

namespace HMS.Essentials.MediatR;

/// <summary>
/// Base class for commands that don't return a value.
/// </summary>
public abstract record CommandBase : ICommand, IRequest<Unit>
{
    /// <summary>
    /// Gets the timestamp when the command was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Base class for commands that return a value.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract record CommandBase<TResponse> : ICommand<TResponse>, IRequest<TResponse>
{
    /// <summary>
    /// Gets the timestamp when the command was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}
