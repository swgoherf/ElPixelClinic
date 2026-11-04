namespace Profiles.Application.Common.Interfaces;

public interface ISpecializationServiceReplicaRepository
{
    Task<IEnumerable<string>> GetServicesBySpecializationAsync(string specializationName);
    Task<bool> SpecializationExistsAsync(string specializationName);
    Task<bool> HasAnyAsync();
    Task AddAsync(string specializationName, string serviceName);
    Task DeleteAsync(string specializationName, string serviceName);
}
