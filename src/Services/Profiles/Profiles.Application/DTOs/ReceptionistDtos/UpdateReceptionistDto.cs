namespace Profiles.Application.DTOs.ReceptionistDtos;

public record UpdateReceptionistDto(
    string? PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    Guid OfficeId
);
