namespace Profiles.Application.DTOs.DoctorDtos;

public record DoctorFilterParameters(
    string? FullName = null,
    string? Specialization = null,
    Guid? OfficeId = null,
    bool AtWorkOnly = false
);
