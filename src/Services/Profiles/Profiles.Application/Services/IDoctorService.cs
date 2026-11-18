using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Domain.Enums;

namespace Profiles.Application.Services;

public interface IDoctorService
{
    Task<IEnumerable<DoctorCardResponseDto>> GetCardsAsync(DoctorFilterParameters filters);
    Task<DoctorDetailResponseDto> GetByIdForPatientAsync(Guid id);
    Task<DoctorAdminResponseDto> GetByIdForAdminAsync(Guid id);
    Task<DoctorOwnerResponseDto> GetByIdForOwnerAsync(Guid id);
    Task<IEnumerable<DoctorAdminResponseDto>> GetAdminListAsync(DoctorFilterParameters filters);
    Task<DoctorAdminResponseDto> CreateAsync(CreateDoctorDto dto);
    Task<DoctorAdminResponseDto> UpdateAsync(Guid id, UpdateDoctorDto dto);
    Task ChangeStatusAsync(Guid id, DoctorStatus status);
    Task<IEnumerable<DoctorOfficeFilterItemDto>> GetOfficesFilterAsync(string? specialization = null, bool atWorkOnly = false);
    Task<IEnumerable<string>> GetSpecializationsFilterAsync(Guid? officeId = null, bool atWorkOnly = false);
}
