using Microsoft.AspNetCore.Mvc;
using Office.Application.DTOs;
using Office.Application.Services;

namespace Office.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OfficesController : ControllerBase
{
    private readonly IOfficeService _officeService;

    public OfficesController(IOfficeService officeService)
    {
        _officeService = officeService;
    }

    // GET: api/Offices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfficeResponseDto>>> GetOffices()
    {
        var offices = await _officeService.GetAllAsync();

        return Ok(offices);
    }

    // GET: api/Offices/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<OfficeResponseDto>> GetOffice(Guid id)
    {
        var office = await _officeService.GetByIdAsync(id);

        if (office == null)
        {
            return NotFound();
        }

        return Ok(office);
    }

    // PUT: api/Offices/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOffice(Guid id, UpdateOfficeDto dto)
    {
        var updated = await _officeService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Offices
    [HttpPost]
    public async Task<ActionResult<OfficeResponseDto>> CreateOffice(CreateOfficeDto dto)
    {
        var created = await _officeService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetOffice),
            new { id = created.Id },
            created
        );
    }

    // DELETE: api/Offices/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOffice(Guid id)
    {
        var deleted = await _officeService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
