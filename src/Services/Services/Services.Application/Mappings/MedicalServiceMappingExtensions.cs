using Services.Application.DTOs.MedicalServiceDtos;
using Services.Domain.Entities;

namespace Services.Application.Mappings;

public static class MedicalServiceMappingExtensions
{
    public static MedicalServiceResponseDto ToResponseDto(this MedicalServiceEntity service)
    {
        return new MedicalServiceResponseDto(
            service.Id,
            service.Name,
            service.Price,
            service.IsActive,
            service.SpecializationId
        );
    }
}
