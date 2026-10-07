using Microsoft.EntityFrameworkCore;
using Office.Api.Models;

namespace Office.Api.Data;

public class OfficeDbContext : DbContext
{
    public OfficeDbContext(DbContextOptions<OfficeDbContext> options) : base(options)
    {
    }

    public DbSet<OfficeEntity> Offices { get; set; }
}

