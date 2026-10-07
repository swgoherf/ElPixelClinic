using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Api.Data;
using Services.Api.DTOs.ServiceDtos;
using Services.Api.Models;

namespace Services.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly ServicesDbContext _context;

    public ServicesController(ServicesDbContext context)
    {
        _context = context;
    }

    // GET: api/Services
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponseDto>>> GetServices()
    {
        var services = await _context.Services.ToListAsync();

        return Ok(services.Select(o => new ServiceResponseDto(
            o.Id,
            o.Name,
            o.Price,
            o.IsActive,
            o.SpecializationId
        )));
    }

    // GET: api/Services/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseDto>> GetService(Guid id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        return Ok(new ServiceResponseDto(
            service.Id,
            service.Name,
            service.Price,
            service.IsActive,
            service.SpecializationId
        ));
    }

    // PUT: api/Services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        service.Name = dto.Name;
        service.Price = dto.Price;
        service.IsActive = dto.IsActive;
        service.SpecializationId = dto.SpecializationId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Services
    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> CreateService(CreateServiceDto dto)
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

        return CreatedAtAction(
            nameof(GetService),
            new { id = entity.Id },
            response
        );
    }

    // DELETE: api/Services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        _context.Services.Remove(service);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
