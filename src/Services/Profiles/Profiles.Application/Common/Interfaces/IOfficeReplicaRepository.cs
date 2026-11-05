namespace Profiles.Application.Common.Interfaces;

public interface IOfficeReplicaRepository
{
    Task<IEnumerable<Profiles.Domain.Entities.OfficeReplicaEntity>> GetAllAsync();
    Task<Profiles.Domain.Entities.OfficeReplicaEntity?> GetByIdAsync(Guid id);
    Task UpsertAsync(Profiles.Domain.Entities.OfficeReplicaEntity office);
    Task UpdateStatusAsync(Guid id, bool isActive);
    Task DeleteAsync(Guid id);
}
