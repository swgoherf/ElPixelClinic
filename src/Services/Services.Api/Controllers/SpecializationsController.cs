using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Api.Data;
using Services.Api.DTOs.SpecializationDtos;
using Services.Api.Models;

namespace Services.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecializationsController : ControllerBase
{
    private readonly ServicesDbContext _context;

    public SpecializationsController(ServicesDbContext context)
    {
        _context = context;
    }

    // GET: api/Specializations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecializationResponseDto>>> GetSpecializations()
    {
        var specializations = await _context.Specializations.ToListAsync();

        return Ok(specializations.Select(s => new SpecializationResponseDto(
            s.Id,
            s.Name
        )));
    }

    // GET: api/Specializations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationResponseDto>> GetSpecialization(Guid id)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null)
        {
            return NotFound();
        }

        return Ok(new SpecializationResponseDto(
            specialization.Id,
            specialization.Name
        ));
    }

    // PUT: api/Specializations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialization(Guid id, UpdateSpecializationDto dto)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null)
        {
            return NotFound();
        }

        specialization.Name = dto.Name;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Specializations
    [HttpPost]
    public async Task<ActionResult<SpecializationResponseDto>> CreateSpecialization(CreateSpecializationDto dto)
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

        return CreatedAtAction(
            nameof(GetSpecialization),
            new { id = entity.Id },
            response
        );
    }

    // DELETE: api/Specializations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecialization(Guid id)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null)
        {
            return NotFound();
        }

        _context.Specializations.Remove(specialization);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
