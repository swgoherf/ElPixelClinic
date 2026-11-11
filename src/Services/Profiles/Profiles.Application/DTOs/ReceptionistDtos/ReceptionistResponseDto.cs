namespace Profiles.Application.DTOs.ReceptionistDtos;

public record ReceptionistResponseDto(
    Guid Id,
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string FullName,
    string Email,
    Guid OfficeId,
    string OfficeAddress
);
