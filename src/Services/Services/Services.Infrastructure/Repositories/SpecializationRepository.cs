using Microsoft.EntityFrameworkCore;
using Services.Application.Common.Interfaces;
using Services.Domain.Entities;
using Services.Infrastructure.Data;

namespace Services.Infrastructure.Repositories;

public class SpecializationRepository : ISpecializationRepository
{
    private readonly ServicesDbContext _context;

    public SpecializationRepository(ServicesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SpecializationEntity>> GetAllAsync()
    {
        return await _context.Specializations.ToListAsync();
    }

    public async Task<SpecializationEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Specializations.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Specializations.AnyAsync(s => s.Id == id);
    }

    public async Task AddAsync(SpecializationEntity specialization)
    {
        await _context.Specializations.AddAsync(specialization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SpecializationEntity specialization)
    {
        _context.Specializations.Update(specialization);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SpecializationEntity specialization)
    {
        _context.Specializations.Remove(specialization);
        await _context.SaveChangesAsync();
    }
}
