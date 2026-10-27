using Services.Application.DTOs.SpecializationDtos;

namespace Services.Application.Services;

public interface ISpecializationService
{
    Task<IEnumerable<SpecializationResponseDto>> GetAllAsync();
    Task<SpecializationResponseDto> GetByIdAsync(Guid id);
    Task<SpecializationResponseDto> UpdateAsync(Guid id, UpdateSpecializationDto dto);
    Task<SpecializationResponseDto> CreateAsync(CreateSpecializationDto dto);
    Task DeleteAsync(Guid id);
}
