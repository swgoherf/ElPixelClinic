using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Office.Api.Data;
using Office.Api.DTOs;
using Office.Api.Models;

namespace Office.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OfficesController : ControllerBase
{
    private readonly OfficeDbContext _context;
    public OfficesController(OfficeDbContext context)
    {
        _context = context;
    }

    // GET: api/OfficeEntity
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfficeResponseDto>>> GetOffices()
    {
        var offices = await _context.Offices.ToListAsync();

        return Ok(offices.Select(o => new OfficeResponseDto(
            o.Id,
            o.Address,
            o.RegistryPhoneNumber,
            o.IsActive
        )));
    }

    // GET: api/OfficeEntity/id
    [HttpGet("{id}")]
    public async Task<ActionResult<OfficeResponseDto>> GetOffice(Guid id)
    {
        var office = await _context.Offices.FindAsync(id);

        if (office == null)
        {
            return NotFound();
        }

        return Ok(new OfficeResponseDto(
            office.Id,
            office.Address,
            office.RegistryPhoneNumber,
            office.IsActive
        ));
    }

    // PUT: api/OfficeEntity/id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOffice(Guid id, UpdateOfficeDto dto)
    {
        var office = await _context.Offices.FindAsync(id);

        if (office == null)
        {
            return NotFound();
        }

        office.Address = dto.Address;
        office.RegistryPhoneNumber = dto.RegistryPhoneNumber;
        office.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/OfficeEntity
    [HttpPost]
    public async Task<ActionResult<OfficeResponseDto>> CreateOffice(CreateOfficeDto dto)
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

        return CreatedAtAction(
            nameof(GetOffice),
            new { id = entity.Id },
            response
        );
    }

    // DELETE: api/OfficeEntity/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOffice(Guid id)
    {
        var office = await _context.Offices.FindAsync(id);

        if (office == null)
        {
            return NotFound();
        }

        _context.Offices.Remove(office);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
