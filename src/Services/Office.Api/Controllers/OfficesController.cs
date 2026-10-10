using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Office.Api.Data;
using Office.Api.DTOs;
using Office.Api.Models;
using Office.Api.Services;

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

    // GET: api/OfficeEntity
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfficeResponseDto>>> GetOffices()
    {
        var offices = await _officeService.GetAllAsync();

        return Ok(offices);
    }

    // GET: api/OfficeEntity/id
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

    // PUT: api/OfficeEntity/id
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

    // POST: api/OfficeEntity
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

    // DELETE: api/OfficeEntity/id
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
