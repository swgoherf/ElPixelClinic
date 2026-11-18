using Profiles.Domain.Entities;

namespace Profiles.Application.Common.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<PatientEntity>> GetAllAsync(string? fullName = null);
    Task<IEnumerable<PatientEntity>> GetUnlinkedAsync();
    Task<IEnumerable<PatientEntity>> FindUnlinkedCandidatesAsync(string firstName, string lastName, DateOnly dateOfBirth);
    Task<PatientEntity?> GetByIdAsync(Guid id);
    Task<PatientEntity?> GetByAccountIdAsync(Guid accountId);
    Task AddAsync(PatientEntity patient);
    Task UpdateAsync(PatientEntity patient);
    Task MarkAsLinkedAsync(Guid id);
    Task LinkToAccountAsync(Guid patientId, Guid accountId);
    Task DeleteAsync(Guid id);
}
