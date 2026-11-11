using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Domain.Entities;
using Profiles.Domain.Enums;

namespace Profiles.Application.Common.Interfaces;

public interface IDoctorRepository
{
    Task<IEnumerable<DoctorEntity>> GetAllAsync(string? fullName = null, string? specialization = null, Guid? officeId = null, bool atWorkOnly = false);
    Task<DoctorEntity?> GetByIdAsync(Guid id);
    Task<DoctorEntity?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, Guid? excludeDoctorId = null);
    Task AddAsync(DoctorEntity doctor);
    Task UpdateAsync(DoctorEntity doctor);
    Task UpdateStatusAsync(Guid id, DoctorStatus status);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<DoctorOfficeFilterItemDto>> GetOfficesBySpecializationAsync(string? specialization = null, bool atWorkOnly = false);
    Task<IEnumerable<string>> GetSpecializationsByOfficeAsync(Guid? officeId = null, bool atWorkOnly = false);
}
