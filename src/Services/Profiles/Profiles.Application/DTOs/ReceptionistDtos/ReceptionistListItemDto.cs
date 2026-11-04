namespace Profiles.Application.DTOs.ReceptionistDtos;

public record ReceptionistListItemDto(
    Guid Id,
    string FullName,
    Guid OfficeId,
    string OfficeAddress
);
