namespace Profiles.Application.Common.Events;

public record PatientProfileDeletedEvent(
    Guid PatientId
);
