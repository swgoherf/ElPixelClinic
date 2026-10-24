using Microsoft.EntityFrameworkCore;
using Office.Domain.Entities;

namespace Office.Infrastructure.Data;

public class OfficeDbContext : DbContext
{
    public OfficeDbContext(DbContextOptions<OfficeDbContext> options) : base(options)
    {
    }

    public DbSet<OfficeEntity> Offices { get; set; }
}
