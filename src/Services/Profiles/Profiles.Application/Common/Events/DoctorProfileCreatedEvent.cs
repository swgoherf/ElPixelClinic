namespace Profiles.Application.Common.Events;

public record DoctorProfileCreatedEvent(
    Guid DoctorId,
    string Email,
    string FirstName,
    string LastName
);
