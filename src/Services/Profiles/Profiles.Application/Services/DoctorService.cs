using Profiles.Application.Common.Exceptions;
using Profiles.Application.Common.Helpers;
using Profiles.Application.Common.Interfaces;
using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Application.Mappings;
using Profiles.Domain.Entities;
using Profiles.Domain.Enums;

namespace Profiles.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IOfficeReplicaRepository _officeRepository;
    private readonly ISpecializationServiceReplicaRepository _specializationServiceRepository;
    private readonly IRegisteredEmailReplicaRepository _registeredEmailRepository;
    private readonly IProfileEventPublisher _eventPublisher;
    private readonly ICurrentUserService _currentUserService;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IOfficeReplicaRepository officeRepository,
        ISpecializationServiceReplicaRepository specializationServiceRepository,
        IRegisteredEmailReplicaRepository registeredEmailRepository,
        IProfileEventPublisher eventPublisher,
        ICurrentUserService currentUserService)
    {
        _doctorRepository = doctorRepository;
        _officeRepository = officeRepository;
        _specializationServiceRepository = specializationServiceRepository;
        _registeredEmailRepository = registeredEmailRepository;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<DoctorCardResponseDto>> GetCardsAsync(DoctorFilterParameters filters)
    {
        var doctors = await _doctorRepository.GetAllAsync(
            fullName: filters.FullName,
            specialization: filters.Specialization,
            officeId: filters.OfficeId,
            atWorkOnly: true
        );

        var offices = await _officeRepository.GetAddressDictionaryAsync();

        return doctors.Select(doctor =>
        {
            offices.TryGetValue(doctor.OfficeId, out var address);
            return doctor.ToCardResponseDto(address);
        });
    }

    public async Task<DoctorDetailResponseDto> GetByIdForPatientAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null)
        {
            throw new NotFoundException($"Doctor with ID '{id}' was not found.");
        }

        var office = await _officeRepository.GetByIdAsync(doctor.OfficeId);
        var services = await _specializationServiceRepository.GetServicesBySpecializationAsync(doctor.Specialization);

        return doctor.ToDetailResponseDto(office?.Address, services.ToList());
    }

    public async Task<DoctorAdminResponseDto> GetByIdForAdminAsync(Guid id)
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can view doctor administration profiles.");
        }

        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null)
        {
            throw new NotFoundException($"Doctor with ID '{id}' was not found.");
        }

        var office = await _officeRepository.GetByIdAsync(doctor.OfficeId);
        return doctor.ToAdminResponseDto(office?.Address);
    }

    public async Task<DoctorOwnerResponseDto> GetByIdForOwnerAsync(Guid id)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (!_currentUserService.IsReceptionistOrAdmin && _currentUserService.UserId != id)
        {
            throw new ForbiddenException("You can only view your own doctor profile.");
        }

        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null)
        {
            throw new NotFoundException($"Doctor with ID '{id}' was not found.");
        }

        var office = await _officeRepository.GetByIdAsync(doctor.OfficeId);
        return doctor.ToOwnerResponseDto(office?.Address);
    }

    public async Task<IEnumerable<DoctorAdminResponseDto>> GetAdminListAsync(DoctorFilterParameters filters)
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can view the doctor administration list.");
        }

        var doctors = await _doctorRepository.GetAllAsync(
            fullName: filters.FullName,
            specialization: filters.Specialization,
            officeId: filters.OfficeId,
            atWorkOnly: filters.AtWorkOnly
        );

        var offices = await _officeRepository.GetAddressDictionaryAsync();

        return doctors.Select(doctor =>
        {
            offices.TryGetValue(doctor.OfficeId, out var address);
            return doctor.ToAdminResponseDto(address);
        });
    }

    public async Task<DoctorAdminResponseDto> CreateAsync(CreateDoctorDto dto)
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can create doctor profiles.");
        }

        ValidateDoctor(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.CareerStartYear, dto.Email, dto.Specialization, dto.OfficeId);
        await _officeRepository.EnsureExistsAndActiveAsync(dto.OfficeId);
        await EnsureSpecializationValidAsync(dto.Specialization);

        var emailExistsInDoctors = await _doctorRepository.EmailExistsAsync(dto.Email);
        var emailExistsInIdentity = await _registeredEmailRepository.ExistsAsync(dto.Email);
        if (emailExistsInDoctors || emailExistsInIdentity)
        {
            throw new ConflictException("User with this email already exists.");
        }

        var entity = new DoctorEntity
        {
            PhotoUrl = dto.PhotoUrl,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email.Trim().ToLowerInvariant(),
            Specialization = dto.Specialization.Trim(),
            OfficeId = dto.OfficeId,
            CareerStartYear = dto.CareerStartYear,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _doctorRepository.AddAsync(entity);

        await _eventPublisher.PublishDoctorProfileCreatedAsync(entity.Id, entity.Email, entity.FirstName, entity.LastName);

        var office = await _officeRepository.GetByIdAsync(entity.OfficeId);
        return entity.ToAdminResponseDto(office?.Address);
    }

    public async Task<DoctorAdminResponseDto> UpdateAsync(Guid id, UpdateDoctorDto dto)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (!_currentUserService.IsReceptionistOrAdmin && _currentUserService.UserId != id)
        {
            throw new ForbiddenException("You can only edit your own doctor profile.");
        }

        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null)
        {
            throw new NotFoundException($"Doctor with ID '{id}' was not found.");
        }

        ValidateDoctor(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.CareerStartYear, dto.Email, dto.Specialization, dto.OfficeId);
        await _officeRepository.EnsureExistsAndActiveAsync(dto.OfficeId);
        await EnsureSpecializationValidAsync(dto.Specialization);

        var isEmailChanged = !string.Equals(doctor.Email, dto.Email, StringComparison.OrdinalIgnoreCase);
        if (isEmailChanged)
        {
            var emailExistsInDoctors = await _doctorRepository.EmailExistsAsync(dto.Email, excludeDoctorId: id);
            var emailExistsInIdentity = await _registeredEmailRepository.ExistsAsync(dto.Email);
            if (emailExistsInDoctors || emailExistsInIdentity)
            {
                throw new ConflictException("User with this email already exists.");
            }
        }

        doctor.PhotoUrl = dto.PhotoUrl;
        doctor.FirstName = dto.FirstName.Trim();
        doctor.LastName = dto.LastName.Trim();
        doctor.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
        doctor.DateOfBirth = dto.DateOfBirth;
        doctor.Email = dto.Email.Trim().ToLowerInvariant();
        doctor.Specialization = dto.Specialization.Trim();
        doctor.OfficeId = dto.OfficeId;
        doctor.CareerStartYear = dto.CareerStartYear;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _doctorRepository.UpdateAsync(doctor);

        var office = await _officeRepository.GetByIdAsync(doctor.OfficeId);
        return doctor.ToAdminResponseDto(office?.Address);
    }

    public async Task ChangeStatusAsync(Guid id, DoctorStatus status)
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can change doctor status.");
        }

        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null)
        {
            throw new NotFoundException($"Doctor with ID '{id}' was not found.");
        }

        await _doctorRepository.UpdateStatusAsync(id, status);
    }

    public async Task<IEnumerable<DoctorOfficeFilterItemDto>> GetOfficesFilterAsync(string? specialization = null, bool atWorkOnly = false)
    {
        return await _doctorRepository.GetOfficesBySpecializationAsync(specialization, atWorkOnly);
    }

    public async Task<IEnumerable<string>> GetSpecializationsFilterAsync(Guid? officeId = null, bool atWorkOnly = false)
    {
        return await _doctorRepository.GetSpecializationsByOfficeAsync(officeId, atWorkOnly);
    }

    private async Task EnsureSpecializationValidAsync(string specialization)
    {
        if (await _specializationServiceRepository.HasAnyAsync())
        {
            var exists = await _specializationServiceRepository.SpecializationExistsAsync(specialization);
            if (!exists)
            {
                throw new ValidationException("Invalid specialization name.");
            }
        }
    }

    private static void ValidateDoctor(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        int careerStartYear,
        string email,
        string specialization,
        Guid officeId)
    {
        PersonValidationHelper.ValidateName(firstName, lastName);

        if (string.IsNullOrWhiteSpace(specialization))
            throw new ValidationException("Specialization is required.");

        PersonValidationHelper.ValidateOfficeId(officeId);
        PersonValidationHelper.ValidateEmail(email);
        PersonValidationHelper.ValidateDateOfBirth(dateOfBirth);

        var currentYear = DateTime.UtcNow.Year;
        if (careerStartYear > currentYear)
            throw new ValidationException("Career start year cannot be in the future.");

        if (careerStartYear < dateOfBirth.Year + 16)
            throw new ValidationException("Career start year cannot be earlier than 16 years from date of birth.");
    }
}
