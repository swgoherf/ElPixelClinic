namespace ElPixelClinic.Contracts.Events;

public record UserEmailUnregisteredEvent(
    Guid UserId,
    string Email
);
