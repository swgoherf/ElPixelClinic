namespace Profiles.Application.Common.Events;

public record ReceptionistProfileCreatedEvent(
    Guid ReceptionistId,
    string Email,
    string FirstName,
    string LastName
);
