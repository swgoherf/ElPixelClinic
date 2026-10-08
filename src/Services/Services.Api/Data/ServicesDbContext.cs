using Microsoft.EntityFrameworkCore;
using Services.Api.Models;

namespace Services.Api.Data;

public class ServicesDbContext : DbContext
{
    public ServicesDbContext(DbContextOptions<ServicesDbContext> options) : base(options)
    {
    }

    public DbSet<SpecializationEntity> Specializations { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ServiceEntity>()
            .HasOne(s => s.Specialization)
            .WithMany(sp => sp.Services)
            .HasForeignKey(s => s.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

