using Office.Application.DTOs;
using Office.Domain.Entities;

namespace Office.Application.Mappings;

public static class OfficeMappingExtensions
{
    public static OfficeResponseDto ToResponseDto(this OfficeEntity office)
    {
        return new OfficeResponseDto(
            office.Id,
            office.Address,
            office.RegistryPhoneNumber,
            office.IsActive
        );
    }
}
