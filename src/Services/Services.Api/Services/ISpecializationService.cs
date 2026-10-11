using Services.Api.DTOs.SpecializationDtos;

namespace Services.Api.Services;

public interface ISpecializationService
{
    Task<IEnumerable<SpecializationResponseDto>> GetAllAsync();
    Task<SpecializationResponseDto?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Guid id, UpdateSpecializationDto dto);
    Task<SpecializationResponseDto> CreateAsync(CreateSpecializationDto dto);
    Task<bool> DeleteAsync(Guid id);
}
