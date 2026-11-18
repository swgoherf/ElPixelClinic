namespace Profiles.Application.DTOs.ReceptionistDtos;

public record CreateReceptionistDto(
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    Guid OfficeId
);
