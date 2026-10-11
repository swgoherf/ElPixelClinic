using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Api.Data;
using Services.Api.DTOs.ServiceDtos;
using Services.Api.Models;
using Services.Api.Services;

namespace Services.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    // GET: api/Services
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponseDto>>> GetServices()
    {
        var services = await _serviceService.GetAllAsync();

        return Ok(services);
    }

    // GET: api/Services/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseDto>> GetService(Guid id)
    {
        var service = await _serviceService.GetByIdAsync(id);

        return Ok(service);
    }

    // PUT: api/Services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateServiceDto dto)
    {
        var updated = await _serviceService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Services
    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> CreateService(CreateServiceDto dto)
    {
        var created = await _serviceService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetService),
            new { id = created.Id },
            created
        );
    }

    // DELETE: api/Services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var deleted = await _serviceService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
