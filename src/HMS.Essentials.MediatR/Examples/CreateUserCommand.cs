using HMS.Essentials.MediatR;

namespace HMS.Essentials.Examples;

/// <summary>
/// Example command that creates a user.
/// This command returns a Result with the created user's ID.
/// </summary>
public record CreateUserCommand(string Email, string Name) : CommandBase<Result<Guid>>;

/// <summary>
/// Example handler for CreateUserCommand.
/// Demonstrates basic command handling with validation and result pattern.
/// </summary>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Simulate validation
        if (string.IsNullOrWhiteSpace(command.Email))
            return Result<Guid>.Failure("Email is required");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<Guid>.Failure("Name is required");

        // Simulate user creation
        await Task.Delay(10, cancellationToken); // Simulating database operation

        var userId = Guid.NewGuid();
        
        return Result<Guid>.Success(userId);
    }
}
