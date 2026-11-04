using Profiles.Application.Common.Exceptions;
using Profiles.Application.Common.Helpers;
using Profiles.Application.Common.Interfaces;
using Profiles.Application.DTOs.PatientDtos;
using Profiles.Application.Mappings;
using Profiles.Domain.Entities;

namespace Profiles.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorPatientAccessRepository _accessRepository;
    private readonly IProfileEventPublisher _eventPublisher;

    public PatientService(
        IPatientRepository patientRepository,
        IDoctorPatientAccessRepository accessRepository,
        IProfileEventPublisher eventPublisher)
    {
        _patientRepository = patientRepository;
        _accessRepository = accessRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<PatientMatchCheckResultDto> CreateSelfAsync(CreatePatientSelfDto dto)
    {
        ValidateSelfPatient(dto);

        var unlinkedProfiles = await _patientRepository.GetUnlinkedAsync();
        foreach (var candidate in unlinkedProfiles)
        {
            var score = CalculateMatchScore(dto, candidate);
            if (score >= 13)
            {
                return new PatientMatchCheckResultDto(
                    HasMatch: true,
                    ExistingProfileId: candidate.Id,
                    MatchedProfile: candidate.ToPersonalInfoDto(),
                    CreatedProfile: null
                );
            }
        }

        var newEntity = new PatientEntity
        {
            PhotoUrl = dto.PhotoUrl,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            DateOfBirth = dto.DateOfBirth,
            IsLinkedToAccount = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _patientRepository.AddAsync(newEntity);

        return new PatientMatchCheckResultDto(
            HasMatch: false,
            ExistingProfileId: null,
            MatchedProfile: null,
            CreatedProfile: newEntity.ToPersonalInfoDto()
        );
    }

    public async Task<PatientPersonalInfoDto> ForceCreateSelfAsync(CreatePatientSelfDto dto)
    {
        ValidateSelfPatient(dto);

        var entity = new PatientEntity
        {
            PhotoUrl = dto.PhotoUrl,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            DateOfBirth = dto.DateOfBirth,
            IsLinkedToAccount = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _patientRepository.AddAsync(entity);
        return entity.ToPersonalInfoDto();
    }

    public async Task<PatientPersonalInfoDto> ConfirmMatchAsync(Guid existingPatientId)
    {
        var patient = await _patientRepository.GetByIdAsync(existingPatientId);
        if (patient is null)
        {
            throw new NotFoundException($"Patient with ID '{existingPatientId}' was not found.");
        }

        patient.IsLinkedToAccount = true;
        patient.UpdatedAt = DateTime.UtcNow;
        await _patientRepository.MarkAsLinkedAsync(existingPatientId);

        return patient.ToPersonalInfoDto();
    }

    public async Task<PatientPersonalInfoDto> CreateByAdminAsync(CreatePatientByAdminDto dto)
    {
        ValidateCommonFields(dto.FirstName, dto.LastName, dto.DateOfBirth);

        var entity = new PatientEntity
        {
            PhotoUrl = null,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
            PhoneNumber = null,
            DateOfBirth = dto.DateOfBirth,
            IsLinkedToAccount = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _patientRepository.AddAsync(entity);
        return entity.ToPersonalInfoDto();
    }

    public async Task<PatientPersonalInfoDto> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null)
        {
            throw new NotFoundException($"Patient with ID '{id}' was not found.");
        }

        ValidateCommonFields(dto.FirstName, dto.LastName, dto.DateOfBirth);

        patient.PhotoUrl = dto.PhotoUrl;
        patient.FirstName = dto.FirstName.Trim();
        patient.LastName = dto.LastName.Trim();
        patient.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
        patient.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();
        patient.DateOfBirth = dto.DateOfBirth;
        patient.UpdatedAt = DateTime.UtcNow;

        await _patientRepository.UpdateAsync(patient);
        return patient.ToPersonalInfoDto();
    }

    public async Task<PatientPersonalInfoDto> GetPersonalInfoAsync(Guid id, Guid? requestingDoctorId = null, bool isReceptionist = false)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null)
        {
            throw new NotFoundException($"Patient with ID '{id}' was not found.");
        }

        if (requestingDoctorId.HasValue)
        {
            var hasAccess = await _accessRepository.HasAccessAsync(requestingDoctorId.Value, id);
            if (!hasAccess)
            {
                throw new ForbiddenException("Access denied. Doctor must have an approved appointment with this patient.");
            }
        }

        return patient.ToPersonalInfoDto();
    }

    public async Task<IEnumerable<PatientAppointmentResultDto>> GetAppointmentResultsAsync(Guid id, Guid? requestingDoctorId = null, bool isReceptionist = false)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null)
        {
            throw new NotFoundException($"Patient with ID '{id}' was not found.");
        }

        if (isReceptionist)
        {
            throw new ForbiddenException("Receptionists are not allowed to view patient appointment results.");
        }

        if (requestingDoctorId.HasValue)
        {
            var hasAccess = await _accessRepository.HasAccessAsync(requestingDoctorId.Value, id);
            if (!hasAccess)
            {
                throw new ForbiddenException("Access denied. Doctor must have an approved appointment with this patient.");
            }
        }

        return Array.Empty<PatientAppointmentResultDto>();
    }

    public async Task<PatientFullProfileDto> GetByIdAsync(Guid id, Guid? requestingDoctorId = null, bool isReceptionist = false)
    {
        var personalInfo = await GetPersonalInfoAsync(id, requestingDoctorId, isReceptionist);
        var appointmentResults = isReceptionist ? null : await GetAppointmentResultsAsync(id, requestingDoctorId, isReceptionist);

        return new PatientFullProfileDto(
            PersonalInfo: personalInfo,
            AppointmentResults: appointmentResults?.ToList()
        );
    }

    public async Task<IEnumerable<PatientListItemDto>> GetListAsync(string? fullNameSearch = null)
    {
        var patients = await _patientRepository.GetAllAsync(fullNameSearch);
        return patients.Select(patient => patient.ToListItemDto());
    }

    public async Task DeleteAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null)
        {
            throw new NotFoundException($"Patient with ID '{id}' was not found.");
        }

        await _patientRepository.DeleteAsync(id);
        await _eventPublisher.PublishPatientProfileDeletedAsync(id);
    }

    private static int CalculateMatchScore(CreatePatientSelfDto dto, PatientEntity candidate)
    {
        var score = 0;

        if (!string.IsNullOrWhiteSpace(dto.FirstName) &&
            string.Equals(dto.FirstName.Trim(), candidate.FirstName?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            score += 5;
        }

        if (!string.IsNullOrWhiteSpace(dto.LastName) &&
            string.Equals(dto.LastName.Trim(), candidate.LastName?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            score += 5;
        }

        if (!string.IsNullOrWhiteSpace(dto.MiddleName) &&
            !string.IsNullOrWhiteSpace(candidate.MiddleName) &&
            string.Equals(dto.MiddleName.Trim(), candidate.MiddleName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            score += 5;
        }

        if (dto.DateOfBirth == candidate.DateOfBirth)
        {
            score += 3;
        }

        return score;
    }

    private static void ValidateSelfPatient(CreatePatientSelfDto dto)
    {
        ValidateCommonFields(dto.FirstName, dto.LastName, dto.DateOfBirth);

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            throw new ValidationException("Phone number is required.");

        if (!dto.PhoneNumber.Trim().StartsWith("+"))
            throw new ValidationException("Phone number must start with '+'.");
    }

    private static void ValidateCommonFields(string firstName, string lastName, DateOnly dateOfBirth)
    {
        PersonValidationHelper.ValidateName(firstName, lastName);
        PersonValidationHelper.ValidateDateOfBirth(dateOfBirth);
    }
}
