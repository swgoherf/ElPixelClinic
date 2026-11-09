using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profiles.Application.Common.Constants;
using Profiles.Application.DTOs.PatientDtos;
using Profiles.Application.Services;

namespace Profiles.Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<ActionResult<IEnumerable<PatientListItemDto>>> GetPatients([FromQuery] string? fullName)
    {
        var patients = await _patientService.GetListAsync(fullName);

        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<PatientFullProfileDto>> GetPatientById(Guid id)
    {
        var profile = await _patientService.GetByIdAsync(id);

        return Ok(profile);
    }

    [HttpGet("{id:guid}/personal-info")]
    [Authorize]
    public async Task<ActionResult<PatientPersonalInfoDto>> GetPatientPersonalInfo(Guid id)
    {
        var personalInfo = await _patientService.GetPersonalInfoAsync(id);

        return Ok(personalInfo);
    }

    [HttpGet("{id:guid}/appointment-results")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PatientAppointmentResultDto>>> GetPatientAppointmentResults(Guid id)
    {
        var results = await _patientService.GetAppointmentResultsAsync(id);

        return Ok(results);
    }

    [HttpPost("self")]
    [Authorize]
    public async Task<ActionResult<PatientMatchCheckResultDto>> CreateSelf([FromBody] CreatePatientSelfDto dto)
    {
        var result = await _patientService.CreateSelfAsync(dto);

        if (result.HasMatch)
        {
            return Ok(result);
        }

        return CreatedAtAction(
            nameof(GetPatientPersonalInfo),
            new { id = result.CreatedProfile!.Id },
            result
        );
    }

    [HttpPost("self/force")]
    [Authorize]
    public async Task<ActionResult<PatientPersonalInfoDto>> ForceCreateSelf([FromBody] CreatePatientSelfDto dto)
    {
        var profile = await _patientService.ForceCreateSelfAsync(dto);

        return CreatedAtAction(nameof(GetPatientPersonalInfo), new { id = profile.Id }, profile);
    }

    [HttpPost("{id:guid}/confirm-match")]
    [Authorize]
    public async Task<ActionResult<PatientPersonalInfoDto>> ConfirmMatch(Guid id)
    {
        var updatedProfile = await _patientService.ConfirmMatchAsync(id);

        return Ok(updatedProfile);
    }

    [HttpPost("admin")]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<ActionResult<PatientPersonalInfoDto>> CreateByAdmin([FromBody] CreatePatientByAdminDto dto)
    {
        var profile = await _patientService.CreateByAdminAsync(dto);

        return CreatedAtAction(nameof(GetPatientPersonalInfo), new { id = profile.Id }, profile);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<PatientPersonalInfoDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientDto dto)
    {
        var updatedProfile = await _patientService.UpdateAsync(id, dto);

        return Ok(updatedProfile);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRoles.ReceptionistOrAdmin)]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        await _patientService.DeleteAsync(id);

        return NoContent();
    }
}
