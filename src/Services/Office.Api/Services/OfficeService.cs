using Microsoft.EntityFrameworkCore;
using Office.Api.Data;
using Office.Api.DTOs;
using Office.Api.Models;

namespace Office.Api.Services;

public class OfficeService : IOfficeService
{
    private readonly OfficeDbContext _context;

    public OfficeService(OfficeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OfficeResponseDto>> GetAllAsync()
    {
        var offices = await _context.Offices.ToListAsync();

        return offices.Select(o => new OfficeResponseDto(
            o.Id,
            o.Address,
            o.RegistryPhoneNumber,
            o.IsActive
        ));
    }

    public async Task<OfficeResponseDto?> GetByIdAsync(Guid id)
    {
        var office = await _context.Offices.FindAsync(id);

        if (office == null)
        {
            return null;
        }

        return new OfficeResponseDto(
            office.Id,
            office.Address,
            office.RegistryPhoneNumber,
            office.IsActive
        );
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateOfficeDto dto)
    {
        var office = await _context.Offices.FindAsync(id);

        if (office == null)
        {
            return false;
        }

        office.Address = dto.Address;
        office.RegistryPhoneNumber = dto.RegistryPhoneNumber;
        office.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<OfficeResponseDto> CreateAsync(CreateOfficeDto dto)
    {
        var entity = new OfficeEntity
        {
            Address = dto.Address,
            RegistryPhoneNumber = dto.RegistryPhoneNumber,
            IsActive = dto.IsActive
        };

        _context.Offices.Add(entity);

        await _context.SaveChangesAsync();

        var response = new OfficeResponseDto(
            entity.Id,
            entity.Address,
            entity.RegistryPhoneNumber,
            entity.IsActive
        );

        return response;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var office = await _context.Offices.FindAsync(id);

        if (office == null)
        {
            return false;
        }

        _context.Offices.Remove(office);

        await _context.SaveChangesAsync();

        return true;
    }


}
