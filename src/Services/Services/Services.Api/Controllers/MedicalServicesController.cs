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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MedicalServiceResponseDto>> GetService(Guid id)
    {
        var service = await _medicalServiceService.GetByIdAsync(id);

        return Ok(service);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateMedicalServiceDto dto)
    {
        await _medicalServiceService.UpdateAsync(id, dto);

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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        await _medicalServiceService.DeleteAsync(id);

        return NoContent();
    }
}
