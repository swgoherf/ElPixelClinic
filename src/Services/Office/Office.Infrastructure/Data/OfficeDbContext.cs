using Microsoft.EntityFrameworkCore;
using Office.Application.Common.Interfaces;
using Office.Domain.Entities;

namespace Office.Infrastructure.Data;

public class OfficeDbContext : DbContext, IOfficeDbContext
{
    public OfficeDbContext(DbContextOptions<OfficeDbContext> options) : base(options)
    {
    }

    public DbSet<OfficeEntity> Offices { get; set; }
}
