using Office.Api.DTOs;

namespace Office.Api.Services;

public interface IOfficeService
{
    Task<IEnumerable<OfficeResponseDto>> GetAllAsync();
    Task<OfficeResponseDto?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Guid id, UpdateOfficeDto dto);
    Task<OfficeResponseDto> CreateAsync(CreateOfficeDto dto);
    Task<bool> DeleteAsync(Guid id);
}
