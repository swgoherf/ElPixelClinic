using Services.Domain.Entities;

namespace Services.Application.Common.Interfaces;

public interface IMedicalServiceRepository
{
    Task<IEnumerable<MedicalServiceEntity>> GetAllAsync();
    Task<MedicalServiceEntity?> GetByIdAsync(Guid id);
    Task AddAsync(MedicalServiceEntity service);
    Task UpdateAsync(MedicalServiceEntity service);
    Task DeleteAsync(MedicalServiceEntity service);
}
