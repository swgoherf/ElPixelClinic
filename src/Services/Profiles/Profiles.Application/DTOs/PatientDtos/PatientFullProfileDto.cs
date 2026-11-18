namespace Profiles.Application.DTOs.PatientDtos;

public record PatientFullProfileDto(
    PatientPersonalInfoDto PersonalInfo,
    IReadOnlyList<PatientAppointmentResultDto>? AppointmentResults = null
);
