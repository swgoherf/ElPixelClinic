namespace Profiles.Application.Common.Interfaces;

public interface IRegisteredEmailReplicaRepository
{
    Task<bool> ExistsAsync(string email);
    Task UpsertAsync(string email, Guid userId);
    Task DeleteAsync(string email);
}
