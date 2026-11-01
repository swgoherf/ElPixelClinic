using Services.Application.DTOs.MedicalServiceDtos;

namespace Services.Application.Services;

public interface IMedicalServiceService
{
    Task<IEnumerable<MedicalServiceResponseDto>> GetAllAsync();
    Task<MedicalServiceResponseDto> GetByIdAsync(Guid id);
    Task<MedicalServiceResponseDto> UpdateAsync(Guid id, UpdateMedicalServiceDto dto);
    Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto dto);
    Task DeleteAsync(Guid id);
}
