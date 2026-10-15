using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;

namespace Services.Application.Common.Interfaces;

public interface IServicesDbContext
{
    DbSet<SpecializationEntity> Specializations { get; }
    DbSet<ServiceEntity> Services { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
