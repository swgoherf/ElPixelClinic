using Services.Application.DTOs.ServiceDtos;

namespace Services.Application.Services;

public interface IMedicalServiceService
{
    Task<IEnumerable<ServiceResponseDto>> GetAllAsync();
    Task<ServiceResponseDto?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Guid id, UpdateServiceDto dto);
    Task<ServiceResponseDto> CreateAsync(CreateServiceDto dto);
    Task<bool> DeleteAsync(Guid id);
}
