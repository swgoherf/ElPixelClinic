namespace Profiles.Application.Common.Events;

public record UserEmailRegisteredEvent(
    Guid UserId,
    string Email
);
