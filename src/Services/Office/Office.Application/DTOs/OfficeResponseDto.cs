namespace Office.Application.DTOs;

public record OfficeResponseDto(
    Guid Id,
    string City,
    string Street,
    string HouseNumber,
    string FullAddress,
    string RegistryPhoneNumber,
    bool IsActive
);
