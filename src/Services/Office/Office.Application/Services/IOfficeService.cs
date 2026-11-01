using Office.Application.DTOs;

namespace Office.Application.Services;

public interface IOfficeService
{
    Task<IEnumerable<OfficeResponseDto>> GetAllAsync();
    Task<OfficeResponseDto> GetByIdAsync(Guid id);
    Task<OfficeResponseDto> UpdateAsync(Guid id, UpdateOfficeDto dto);
    Task<OfficeResponseDto> CreateAsync(CreateOfficeDto dto);
    Task DeleteAsync(Guid id);
}
