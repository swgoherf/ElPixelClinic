namespace Profiles.Application.DTOs.DoctorDtos;

public record UpdateDoctorDto(
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly DateOfBirth,
    string Email,
    string Specialization,
    Guid OfficeId,
    int CareerStartYear
);
