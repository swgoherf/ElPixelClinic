namespace Office.Application.DTOs;

public record OfficeResponseDto(
    Guid Id,
    string Address,
    string RegistryPhoneNumber,
    bool IsActive
);
