namespace ElPixelClinic.Contracts.Events;

public record UserEmailRegisteredEvent(
    Guid UserId,
    string Email
);
