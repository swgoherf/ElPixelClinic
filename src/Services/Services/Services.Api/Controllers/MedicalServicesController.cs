using Microsoft.AspNetCore.Mvc;
using Services.Application.DTOs.MedicalServiceDtos;
using Services.Application.Services;

namespace Services.Api.Controllers;

[Route("api/services")]
[ApiController]
public class MedicalServicesController : ControllerBase
{
    private readonly IMedicalServiceService _medicalServiceService;

    public MedicalServicesController(IMedicalServiceService medicalServiceService)
    {
        _medicalServiceService = medicalServiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalServiceResponseDto>>> GetServices()
    {
        var services = await _medicalServiceService.GetAllAsync();

        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MedicalServiceResponseDto>> GetService(Guid id)
    {
        var service = await _medicalServiceService.GetByIdAsync(id);

        if (service is null)
        {
            return NotFound();
        }

        return Ok(service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateMedicalServiceDto dto)
    {
        var updatedService = await _medicalServiceService.UpdateAsync(id, dto);

        if (!updatedService)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<MedicalServiceResponseDto>> CreateService(CreateMedicalServiceDto dto)
    {
        var createdService = await _medicalServiceService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetService),
            new { id = createdService.Id },
            createdService
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var deletedService = await _medicalServiceService.DeleteAsync(id);

        if (!deletedService)
        {
            return NotFound();
        }

        return NoContent();
    }
}
