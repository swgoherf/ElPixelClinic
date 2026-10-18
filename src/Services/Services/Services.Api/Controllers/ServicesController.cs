using Microsoft.AspNetCore.Mvc;
using Services.Application.DTOs.ServiceDtos;
using Services.Application.Services;

namespace Services.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly IMedicalServiceService _serviceService;

    public ServicesController(IMedicalServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponseDto>>> GetServices()
    {
        var services = await _serviceService.GetAllAsync();

        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseDto>> GetService(Guid id)
    {
        var service = await _serviceService.GetByIdAsync(id);

        if (service is null)
        {
            return NotFound();
        }

        return Ok(service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateServiceDto dto)
    {
        var updatedService = await _serviceService.UpdateAsync(id, dto);

        if (!updatedService)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> CreateService(CreateServiceDto dto)
    {
        var createdService = await _serviceService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetService),
            new { id = createdService.Id },
            createdService
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var deletedService = await _serviceService.DeleteAsync(id);

        if (!deletedService)
        {
            return NotFound();
        }

        return NoContent();
    }
}
