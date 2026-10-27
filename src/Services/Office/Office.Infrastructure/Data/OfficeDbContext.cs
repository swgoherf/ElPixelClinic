using Microsoft.EntityFrameworkCore;
using Office.Domain.Entities;

namespace Office.Infrastructure.Data;

public class OfficeDbContext : DbContext
{
    public OfficeDbContext(DbContextOptions<OfficeDbContext> options) : base(options)
    {
    }

    public DbSet<OfficeEntity> Offices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OfficeEntity>(entity =>
        {
            entity.OwnsOne(o => o.Address, address =>
            {
                address.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(150)
                    .IsRequired();

                address.Property(a => a.HouseNumber)
                    .HasColumnName("HouseNumber")
                    .HasMaxLength(20)
                    .IsRequired();
            });
        });
    }
}
