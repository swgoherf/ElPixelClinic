using Profiles.Domain.Entities;

namespace Profiles.Application.Common.Interfaces;

public interface IReceptionistRepository
{
    Task<IEnumerable<ReceptionistEntity>> GetAllAsync();
    Task<ReceptionistEntity?> GetByIdAsync(Guid id);
    Task<ReceptionistEntity?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, Guid? excludeReceptionistId = null);
    Task AddAsync(ReceptionistEntity receptionist);
    Task UpdateAsync(ReceptionistEntity receptionist);
    Task DeleteAsync(Guid id);
}
