namespace Profiles.Application.DTOs.PatientDtos;

public record PatientAppointmentResultDto(
    Guid AppointmentId,
    DateTime Date,
    string DoctorFullName,
    string Specialization,
    string Recommendations,
    string Diagnosis
);
