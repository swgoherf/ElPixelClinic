using Microsoft.EntityFrameworkCore;
using Office.Application.Common.Interfaces;
using Office.Domain.Entities;

namespace Office.Infrastructure.Repositories;

public class OfficeRepository : IOfficeRepository
{
    private readonly IOfficeDbContext _context;

    public OfficeRepository(IOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OfficeEntity>> GetAllAsync()
    {
        return await _context.Offices.ToListAsync();
    }

    public async Task<OfficeEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Offices.FindAsync(id);
    }

    public async Task AddAsync(OfficeEntity office)
    {
        await _context.Offices.AddAsync(office);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OfficeEntity office)
    {
        _context.Offices.Update(office);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OfficeEntity office)
    {
        _context.Offices.Remove(office);
        await _context.SaveChangesAsync();
    }
}
