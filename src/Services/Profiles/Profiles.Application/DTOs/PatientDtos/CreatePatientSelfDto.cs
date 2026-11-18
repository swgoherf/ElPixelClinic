namespace Profiles.Application.DTOs.PatientDtos;

public record CreatePatientSelfDto(
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string PhoneNumber,
    DateOnly DateOfBirth
);
