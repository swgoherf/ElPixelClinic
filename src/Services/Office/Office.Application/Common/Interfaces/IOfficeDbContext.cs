using Microsoft.EntityFrameworkCore;
using Office.Domain.Entities;

namespace Office.Application.Common.Interfaces;

public interface IOfficeDbContext
{
    DbSet<OfficeEntity> Offices { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
