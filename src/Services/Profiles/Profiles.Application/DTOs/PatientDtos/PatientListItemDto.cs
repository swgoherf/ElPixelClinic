namespace Profiles.Application.DTOs.PatientDtos;

public record PatientListItemDto(
    Guid Id,
    string FullName,
    string? PhoneNumber
);
