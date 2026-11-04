using Microsoft.AspNetCore.Mvc;
using Profiles.Application.DTOs.ReceptionistDtos;
using Profiles.Application.Services;

namespace Profiles.Api.Controllers;

[ApiController]
[Route("api/receptionists")]
public class ReceptionistsController : ControllerBase
{
    private readonly IReceptionistService _receptionistService;

    public ReceptionistsController(IReceptionistService receptionistService)
    {
        _receptionistService = receptionistService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReceptionistListItemDto>>> GetReceptionists()
    {
        var receptionists = await _receptionistService.GetListAsync();
        return Ok(receptionists);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReceptionistResponseDto>> GetReceptionistById(Guid id)
    {
        var receptionist = await _receptionistService.GetByIdAsync(id);
        return Ok(receptionist);
    }

    [HttpPost]
    public async Task<ActionResult<ReceptionistResponseDto>> CreateReceptionist([FromBody] CreateReceptionistDto dto)
    {
        var created = await _receptionistService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetReceptionistById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ReceptionistResponseDto>> UpdateReceptionist(Guid id, [FromBody] UpdateReceptionistDto dto)
    {
        var updated = await _receptionistService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteReceptionist(Guid id)
    {
        await _receptionistService.DeleteAsync(id);
        return NoContent();
    }
}
