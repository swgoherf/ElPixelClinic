namespace Office.Api.DTOs;

public record OfficeResponseDto(
    Guid Id,
    string Address,
    string RegistryPhoneNumber,
    bool IsActive
);

