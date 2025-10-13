using HMS.Essentials.MediatR;

namespace HMS.Essentials.Examples;

/// <summary>
/// Example query that retrieves user details by ID.
/// </summary>
public record GetUserByIdQuery(Guid UserId) : QueryBase<Result<UserDto>>;

/// <summary>
/// Data transfer object for user information.
/// </summary>
public record UserDto(Guid Id, string Email, string Name);

/// <summary>
/// Example handler for GetUserByIdQuery.
/// Demonstrates query handling with result pattern.
/// </summary>
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        // Simulate database lookup
        await Task.Delay(10, cancellationToken);

        // Simulate user not found scenario (50% chance)
        if (query.UserId == Guid.Empty)
            return Result<UserDto>.Failure("User not found");

        var userDto = new UserDto(
            query.UserId,
            "user@example.com",
            "John Doe"
        );

        return Result<UserDto>.Success(userDto);
    }
}
