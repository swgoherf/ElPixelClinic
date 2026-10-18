using Services.Application.DTOs.ServiceDtos;
using Services.Domain.Entities;

namespace Services.Application.Mappings;

public static class ServiceMappingExtensions
{
    public static ServiceResponseDto ToResponseDto(this ServiceEntity service)
    {
        return new ServiceResponseDto(
            service.Id,
            service.Name,
            service.Price,
            service.IsActive,
            service.SpecializationId
        );
    }
}
