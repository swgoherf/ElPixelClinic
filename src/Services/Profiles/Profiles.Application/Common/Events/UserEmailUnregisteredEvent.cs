namespace Profiles.Application.Common.Events;

public record UserEmailUnregisteredEvent(
    Guid UserId,
    string Email
);
