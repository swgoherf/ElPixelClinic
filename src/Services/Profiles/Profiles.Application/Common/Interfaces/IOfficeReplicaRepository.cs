using Profiles.Domain.Entities;

namespace Profiles.Application.Common.Interfaces;

public interface IOfficeReplicaRepository
{
    Task<OfficeReplicaEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<OfficeReplicaEntity>> GetAllAsync();
    Task UpsertAsync(OfficeReplicaEntity office);
    Task DeleteAsync(Guid id);
}
