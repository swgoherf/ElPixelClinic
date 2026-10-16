using Microsoft.EntityFrameworkCore;
using Services.Application.Common.Interfaces;
using Services.Application.DTOs.ServiceDtos;
using Services.Domain.Entities;

namespace Services.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServicesDbContext _context;

    public ServiceService(IServicesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceResponseDto>> GetAllAsync()
    {
        var services = await _context.Services.ToListAsync();

        return services.Select(o => new ServiceResponseDto(
            o.Id,
            o.Name,
            o.Price,
            o.IsActive,
            o.SpecializationId
        ));
    }

    public async Task<ServiceResponseDto?> GetByIdAsync(Guid id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return null;
        }

        return new ServiceResponseDto(
            service.Id,
            service.Name,
            service.Price,
            service.IsActive,
            service.SpecializationId
        );
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return false;
        }

        var specializationExists = await _context.Specializations
            .AnyAsync(s => s.Id == dto.SpecializationId);

        if (!specializationExists)
        {
            return false;
        }

        service.Name = dto.Name;
        service.Price = dto.Price;
        service.IsActive = dto.IsActive;
        service.SpecializationId = dto.SpecializationId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ServiceResponseDto> CreateAsync(CreateServiceDto dto)
    {
        var entity = new ServiceEntity
        {
            Name = dto.Name,
            Price = dto.Price,
            IsActive = dto.IsActive,
            SpecializationId = dto.SpecializationId,
        };

        _context.Services.Add(entity);

        await _context.SaveChangesAsync();

        var response = new ServiceResponseDto(
            entity.Id,
            entity.Name,
            entity.Price,
            entity.IsActive,
            entity.SpecializationId
        );

        return response;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return false;
        }

        _context.Services.Remove(service);

        await _context.SaveChangesAsync();

        return true;
    }
}
