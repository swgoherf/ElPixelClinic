namespace Profiles.Application.Common.Events;

public record AppointmentStatusChangedEvent(
    Guid AppointmentId,
    Guid DoctorId,
    Guid PatientId,
    string Status
);
