using Services.Domain.Entities;

namespace Services.Application.Common.Interfaces;

public interface IMedicalServiceRepository
{
    Task<IEnumerable<ServiceEntity>> GetAllAsync();
    Task<ServiceEntity?> GetByIdAsync(Guid id);
    Task AddAsync(ServiceEntity service);
    Task UpdateAsync(ServiceEntity service);
    Task DeleteAsync(ServiceEntity service);
}
