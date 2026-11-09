using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profiles.Application.Common.Constants;
using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Application.Services;

namespace Profiles.Api.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DoctorCardResponseDto>>> GetDoctors(
        [FromQuery] string? fullName,
        [FromQuery] string? specialization,
        [FromQuery] Guid? officeId)
    {
        var filters = new DoctorFilterParameters(
            FullName: fullName,
            Specialization: specialization,
            OfficeId: officeId,
            AtWorkOnly: true
        );

        var doctors = await _doctorService.GetCardsAsync(filters);

        return Ok(doctors);
    }

    [HttpGet("admin")]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<ActionResult<IEnumerable<DoctorAdminResponseDto>>> GetDoctorsForAdmin(
        [FromQuery] string? fullName,
        [FromQuery] string? specialization,
        [FromQuery] Guid? officeId,
        [FromQuery] bool atWorkOnly = false)
    {
        var filters = new DoctorFilterParameters(
            FullName: fullName,
            Specialization: specialization,
            OfficeId: officeId,
            AtWorkOnly: atWorkOnly
        );

        var doctors = await _doctorService.GetAdminListAsync(filters);

        return Ok(doctors);
    }

    [HttpGet("filters/offices")]
    [HttpGet("offices")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DoctorOfficeFilterItemDto>>> GetOfficesFilter(
        [FromQuery] string? specialization,
        [FromQuery] bool atWorkOnly = false)
    {
        var offices = await _doctorService.GetOfficesFilterAsync(specialization, atWorkOnly);

        return Ok(offices);
    }

    [HttpGet("filters/specializations")]
    [HttpGet("specializations")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetSpecializationsFilter(
        [FromQuery] Guid? officeId,
        [FromQuery] bool atWorkOnly = false)
    {
        var specializations = await _doctorService.GetSpecializationsFilterAsync(officeId, atWorkOnly);

        return Ok(specializations);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<DoctorDetailResponseDto>> GetDoctorById(Guid id)
    {
        var doctor = await _doctorService.GetByIdForPatientAsync(id);

        return Ok(doctor);
    }

    [HttpGet("{id:guid}/admin")]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<ActionResult<DoctorAdminResponseDto>> GetDoctorByIdForAdmin(Guid id)
    {
        var doctor = await _doctorService.GetByIdForAdminAsync(id);

        return Ok(doctor);
    }

    [HttpGet("{id:guid}/owner")]
    [Authorize(Roles = UserRoles.DoctorOrReceptionistOrAdmin)]
    public async Task<ActionResult<DoctorOwnerResponseDto>> GetDoctorByIdForOwner(Guid id)
    {
        var doctor = await _doctorService.GetByIdForOwnerAsync(id);

        return Ok(doctor);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<ActionResult<DoctorAdminResponseDto>> CreateDoctor([FromBody] CreateDoctorDto dto)
    {
        var created = await _doctorService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetDoctorByIdForAdmin), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRoles.DoctorOrReceptionistOrAdmin)]
    public async Task<ActionResult<DoctorAdminResponseDto>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto dto)
    {
        var updated = await _doctorService.UpdateAsync(id, dto);

        return Ok(updated);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<IActionResult> ChangeDoctorStatus(Guid id, [FromBody] ChangeDoctorStatusDto dto)
    {
        await _doctorService.ChangeStatusAsync(id, dto.Status);

        return NoContent();
    }
}
