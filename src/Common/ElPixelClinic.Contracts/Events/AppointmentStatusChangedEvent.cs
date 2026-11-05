namespace ElPixelClinic.Contracts.Events;

public record AppointmentStatusChangedEvent(
    Guid AppointmentId,
    Guid DoctorId,
    Guid PatientId,
    string Status
);
