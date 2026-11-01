using Microsoft.EntityFrameworkCore;
using Services.Application.Common.Interfaces;
using Services.Domain.Entities;
using Services.Infrastructure.Data;

namespace Services.Infrastructure.Repositories;

public class MedicalServiceRepository : IMedicalServiceRepository
{
    private readonly ServicesDbContext _context;

    public MedicalServiceRepository(ServicesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MedicalServiceEntity>> GetAllAsync()
    {
        return await _context.MedicalServices.ToListAsync();
    }

    public async Task<MedicalServiceEntity?> GetByIdAsync(Guid id)
    {
        return await _context.MedicalServices.FindAsync(id);
    }

    public async Task AddAsync(MedicalServiceEntity service)
    {
        await _context.MedicalServices.AddAsync(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MedicalServiceEntity service)
    {
        _context.MedicalServices.Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MedicalServiceEntity service)
    {
        _context.MedicalServices.Remove(service);
        await _context.SaveChangesAsync();
    }
}
