using HMS.Essentials.MediatR;

namespace HMS.Essentials.Examples;

/// <summary>
/// Example domain event that is raised when a user is created.
/// </summary>
public record UserCreatedEvent(Guid UserId, string Email, string Name) : DomainEventBase;

/// <summary>
/// Example handler that sends a welcome email when a user is created.
/// Demonstrates multiple handlers for the same domain event.
/// </summary>
public class SendWelcomeEmailHandler : IDomainEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[SendWelcomeEmailHandler] Sending welcome email to {domainEvent.Email}");
        await Task.Delay(10, cancellationToken); // Simulate email sending
        Console.WriteLine($"[SendWelcomeEmailHandler] Welcome email sent to {domainEvent.Email}");
    }
}

/// <summary>
/// Example handler that logs user creation.
/// Demonstrates multiple handlers for the same domain event.
/// </summary>
public class LogUserCreationHandler : IDomainEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[LogUserCreationHandler] Logging user creation: {domainEvent.Name} ({domainEvent.Email})");
        await Task.Delay(5, cancellationToken); // Simulate logging
        Console.WriteLine($"[LogUserCreationHandler] User creation logged with ID: {domainEvent.UserId}");
    }
}

/// <summary>
/// Example handler that creates user profile.
/// Demonstrates multiple handlers for the same domain event.
/// </summary>
public class CreateUserProfileHandler : IDomainEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CreateUserProfileHandler] Creating profile for user: {domainEvent.Name}");
        await Task.Delay(15, cancellationToken); // Simulate profile creation
        Console.WriteLine($"[CreateUserProfileHandler] Profile created for user: {domainEvent.UserId}");
    }
}
