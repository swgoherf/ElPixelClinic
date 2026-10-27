using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;

namespace Services.Infrastructure.Data;

public class ServicesDbContext : DbContext
{
    public ServicesDbContext(DbContextOptions<ServicesDbContext> options) : base(options)
    {
    }

    public DbSet<SpecializationEntity> Specializations { get; set; }
    public DbSet<MedicalServiceEntity> MedicalServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MedicalServiceEntity>()
            .HasOne(s => s.Specialization)
            .WithMany(sp => sp.MedicalServices)
            .HasForeignKey(s => s.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
