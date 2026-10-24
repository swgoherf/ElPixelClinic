using Services.Application.DTOs.MedicalServiceDtos;

namespace Services.Application.Services;

public interface IMedicalServiceService
{
    Task<IEnumerable<MedicalServiceResponseDto>> GetAllAsync();
    Task<MedicalServiceResponseDto?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Guid id, UpdateMedicalServiceDto dto);
    Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto dto);
    Task<bool> DeleteAsync(Guid id);
}
