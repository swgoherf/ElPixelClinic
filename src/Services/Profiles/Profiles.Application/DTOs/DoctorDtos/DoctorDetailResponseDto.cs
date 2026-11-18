namespace Profiles.Application.DTOs.DoctorDtos;

public record DoctorDetailResponseDto(
    Guid Id,
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string FullName,
    string Specialization,
    int Experience,
    Guid OfficeId,
    string OfficeAddress,
    IReadOnlyList<string> Services
);
