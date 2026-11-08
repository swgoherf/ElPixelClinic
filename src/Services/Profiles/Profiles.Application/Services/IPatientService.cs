using Profiles.Application.DTOs.PatientDtos;

namespace Profiles.Application.Services;

public interface IPatientService
{
    Task<PatientMatchCheckResultDto> CreateSelfAsync(CreatePatientSelfDto dto);
    Task<PatientPersonalInfoDto> ForceCreateSelfAsync(CreatePatientSelfDto dto);
    Task<PatientPersonalInfoDto> ConfirmMatchAsync(Guid existingPatientId);
    Task<PatientPersonalInfoDto> CreateByAdminAsync(CreatePatientByAdminDto dto);
    Task<PatientPersonalInfoDto> UpdateAsync(Guid id, UpdatePatientDto dto);
    Task<PatientFullProfileDto> GetByIdAsync(Guid id);
    Task<PatientPersonalInfoDto> GetPersonalInfoAsync(Guid id);
    Task<IEnumerable<PatientAppointmentResultDto>> GetAppointmentResultsAsync(Guid id);
    Task<IEnumerable<PatientListItemDto>> GetListAsync(string? fullNameSearch = null);
    Task DeleteAsync(Guid id);
}
