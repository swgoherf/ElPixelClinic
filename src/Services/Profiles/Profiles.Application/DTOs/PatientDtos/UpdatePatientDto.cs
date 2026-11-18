namespace Profiles.Application.DTOs.PatientDtos;

public record UpdatePatientDto(
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string? PhoneNumber,
    DateOnly DateOfBirth
);
