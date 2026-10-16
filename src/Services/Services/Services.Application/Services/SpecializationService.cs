using Microsoft.EntityFrameworkCore;
using Services.Application.Common.Interfaces;
using Services.Application.DTOs.SpecializationDtos;
using Services.Domain.Entities;

namespace Services.Application.Services;

public class SpecializationService : ISpecializationService
{
    private readonly IServicesDbContext _context;

    public SpecializationService(IServicesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SpecializationResponseDto>> GetAllAsync()
    {
        var specializations = await _context.Specializations.ToListAsync();

        return specializations.Select(s => new SpecializationResponseDto(
            s.Id,
            s.Name
        ));
    }

    public async Task<SpecializationResponseDto?> GetByIdAsync(Guid id)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null)
        {
            return null;
        }

        return new SpecializationResponseDto(
            specialization.Id,
            specialization.Name
        );
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSpecializationDto dto)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null)
        {
            return false;
        }

        specialization.Name = dto.Name;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<SpecializationResponseDto> CreateAsync(CreateSpecializationDto dto)
    {
        var entity = new SpecializationEntity
        {
            Name = dto.Name
        };

        _context.Specializations.Add(entity);

        await _context.SaveChangesAsync();

        var response = new SpecializationResponseDto(
            entity.Id,
            entity.Name
        );

        return response;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null)
        {
            return false;
        }

        _context.Specializations.Remove(specialization);

        await _context.SaveChangesAsync();

        return true;
    }
}
