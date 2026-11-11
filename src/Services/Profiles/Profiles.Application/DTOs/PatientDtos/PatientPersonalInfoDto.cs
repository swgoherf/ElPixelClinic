namespace Profiles.Application.DTOs.PatientDtos;

public record PatientPersonalInfoDto(
    Guid Id,
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string FullName,
    string? PhoneNumber,
    DateOnly DateOfBirth,
    bool IsLinkedToAccount
);
