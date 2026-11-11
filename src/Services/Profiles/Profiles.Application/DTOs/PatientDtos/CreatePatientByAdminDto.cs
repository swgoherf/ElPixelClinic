namespace Profiles.Application.DTOs.PatientDtos;

public record CreatePatientByAdminDto(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly DateOfBirth
);
