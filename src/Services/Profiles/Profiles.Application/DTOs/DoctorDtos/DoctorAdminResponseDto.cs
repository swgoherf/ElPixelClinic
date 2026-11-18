using Profiles.Domain.Enums;

namespace Profiles.Application.DTOs.DoctorDtos;

public record DoctorAdminResponseDto(
    Guid Id,
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string FullName,
    DateOnly DateOfBirth,
    string Email,
    string Specialization,
    Guid OfficeId,
    string OfficeAddress,
    int CareerStartYear,
    int Experience,
    DoctorStatus Status
);
