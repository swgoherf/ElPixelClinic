using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Api.Data;
using Services.Api.DTOs.SpecializationDtos;
using Services.Api.Models;
using Services.Api.Services;

namespace Services.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecializationsController : ControllerBase
{
    private readonly ISpecializationService _specializationService;

    public SpecializationsController(ISpecializationService specializationService)
    {
        _specializationService = specializationService;
    }

    // GET: api/Specializations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecializationResponseDto>>> GetSpecializations()
    {
        var specializations = await _specializationService.GetAllAsync();

        return Ok(specializations);
    }

    // GET: api/Specializations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationResponseDto>> GetSpecialization(Guid id)
    {
        var specialization = await _specializationService.GetByIdAsync(id);

        return Ok(specialization);
    }

    // PUT: api/Specializations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialization(Guid id, UpdateSpecializationDto dto)
    {
        var updated = await _specializationService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Specializations
    [HttpPost]
    public async Task<ActionResult<SpecializationResponseDto>> CreateSpecialization(CreateSpecializationDto dto)
    {
        var created = await _specializationService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetSpecialization),
            new { id = created.Id },
            created
        );
    }

    // DELETE: api/Specializations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecialization(Guid id)
    {
        var deleted = await _specializationService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
