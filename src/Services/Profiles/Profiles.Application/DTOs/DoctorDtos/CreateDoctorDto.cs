using Profiles.Domain.Enums;

namespace Profiles.Application.DTOs.DoctorDtos;

public record CreateDoctorDto(
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly DateOfBirth,
    string Email,
    string Specialization,
    Guid OfficeId,
    int CareerStartYear,
    DoctorStatus Status = DoctorStatus.AtWork
);
