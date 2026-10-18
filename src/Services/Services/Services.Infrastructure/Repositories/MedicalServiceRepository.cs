using Microsoft.EntityFrameworkCore;
using Services.Application.Common.Interfaces;
using Services.Domain.Entities;

namespace Services.Infrastructure.Repositories;

public class MedicalServiceRepository : IMedicalServiceRepository
{
    private readonly IServicesDbContext _context;

    public MedicalServiceRepository(IServicesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceEntity>> GetAllAsync()
    {
        return await _context.Services.ToListAsync();
    }

    public async Task<ServiceEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Services.FindAsync(id);
    }

    public async Task AddAsync(ServiceEntity service)
    {
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceEntity service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ServiceEntity service)
    {
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }
}
