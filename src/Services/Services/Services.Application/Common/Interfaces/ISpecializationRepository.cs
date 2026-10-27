using Services.Domain.Entities;

namespace Services.Application.Common.Interfaces;

public interface ISpecializationRepository
{
    Task<IEnumerable<SpecializationEntity>> GetAllAsync();
    Task<SpecializationEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(SpecializationEntity specialization);
    Task UpdateAsync(SpecializationEntity specialization);
    Task DeleteAsync(SpecializationEntity specialization);
}
