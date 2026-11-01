using Services.Application.DTOs.SpecializationDtos;
using Services.Domain.Entities;

namespace Services.Application.Mappings;

public static class SpecializationMappingExtensions
{
    public static SpecializationResponseDto ToResponseDto(this SpecializationEntity specialization)
    {
        return new SpecializationResponseDto(
            specialization.Id,
            specialization.Name
        );
    }
}
