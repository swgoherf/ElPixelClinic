using Profiles.Domain.Entities;

namespace Profiles.Application.Common.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<PatientEntity>> GetAllAsync(string? fullName = null);
    Task<IEnumerable<PatientEntity>> GetUnlinkedAsync();
    Task<PatientEntity?> GetByIdAsync(Guid id);
    Task AddAsync(PatientEntity patient);
    Task UpdateAsync(PatientEntity patient);
    Task MarkAsLinkedAsync(Guid id);
    Task DeleteAsync(Guid id);
}
