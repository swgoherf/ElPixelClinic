using Microsoft.AspNetCore.Mvc;
using Services.Application.DTOs.SpecializationDtos;
using Services.Application.Services;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecializationResponseDto>>> GetSpecializations()
    {
        var specializations = await _specializationService.GetAllAsync();

        return Ok(specializations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationResponseDto>> GetSpecialization(Guid id)
    {
        var specialization = await _specializationService.GetByIdAsync(id);

        if (specialization is null)
        {
            return NotFound();
        }

        return Ok(specialization);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialization(Guid id, UpdateSpecializationDto dto)
    {
        var updatedSpecialization = await _specializationService.UpdateAsync(id, dto);

        if (!updatedSpecialization)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<SpecializationResponseDto>> CreateSpecialization(CreateSpecializationDto dto)
    {
        var createdSpecialization = await _specializationService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetSpecialization),
            new { id = createdSpecialization.Id },
            createdSpecialization
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecialization(Guid id)
    {
        var deletedSpecialization = await _specializationService.DeleteAsync(id);

        if (!deletedSpecialization)
        {
            return NotFound();
        }

        return NoContent();
    }
}
