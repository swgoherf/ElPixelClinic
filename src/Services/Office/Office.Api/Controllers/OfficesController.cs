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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfficeResponseDto>>> GetOffices()
    {
        var offices = await _officeService.GetAllAsync();

        return Ok(offices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OfficeResponseDto>> GetOffice(Guid id)
    {
        var office = await _officeService.GetByIdAsync(id);

        if (office is null)
        {
            return NotFound();
        }

        return Ok(office);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOffice(Guid id, UpdateOfficeDto dto)
    {
        var updatedOffice = await _officeService.UpdateAsync(id, dto);

        if (!updatedOffice)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<OfficeResponseDto>> CreateOffice(CreateOfficeDto dto)
    {
        var createdOffice = await _officeService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetOffice),
            new { id = createdOffice.Id },
            createdOffice
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOffice(Guid id)
    {
        var deletedOffice = await _officeService.DeleteAsync(id);

        if (!deletedOffice)
        {
            return NotFound();
        }

        return NoContent();
    }
}
