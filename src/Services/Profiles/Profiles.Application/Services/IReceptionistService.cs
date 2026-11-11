using Profiles.Application.DTOs.ReceptionistDtos;

namespace Profiles.Application.Services;

public interface IReceptionistService
{
    Task<IEnumerable<ReceptionistListItemDto>> GetListAsync();
    Task<ReceptionistResponseDto> GetByIdAsync(Guid id);
    Task<ReceptionistResponseDto> CreateAsync(CreateReceptionistDto dto);
    Task<ReceptionistResponseDto> UpdateAsync(Guid id, UpdateReceptionistDto dto);
    Task DeleteAsync(Guid id);
}
