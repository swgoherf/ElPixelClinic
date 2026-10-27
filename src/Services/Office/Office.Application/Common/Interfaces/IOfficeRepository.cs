using Office.Domain.Entities;

namespace Office.Application.Common.Interfaces;

public interface IOfficeRepository
{
    Task<IEnumerable<OfficeEntity>> GetAllAsync();
    Task<OfficeEntity?> GetByIdAsync(Guid id);
    Task AddAsync(OfficeEntity office);
    Task UpdateAsync(OfficeEntity office);
    Task DeleteAsync(OfficeEntity office);
}
