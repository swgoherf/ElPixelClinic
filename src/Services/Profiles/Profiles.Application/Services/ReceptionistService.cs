using Profiles.Application.Common.Exceptions;
using Profiles.Application.Common.Helpers;
using Profiles.Application.Common.Interfaces;
using Profiles.Application.DTOs.ReceptionistDtos;
using Profiles.Application.Mappings;
using Profiles.Domain.Entities;

namespace Profiles.Application.Services;

public class ReceptionistService : IReceptionistService
{
    private readonly IReceptionistRepository _receptionistRepository;
    private readonly IOfficeReplicaRepository _officeRepository;
    private readonly IRegisteredEmailReplicaRepository _registeredEmailRepository;
    private readonly IProfileEventPublisher _eventPublisher;
    private readonly ICurrentUserService _currentUserService;

    public ReceptionistService(
        IReceptionistRepository receptionistRepository,
        IOfficeReplicaRepository officeRepository,
        IRegisteredEmailReplicaRepository registeredEmailRepository,
        IProfileEventPublisher eventPublisher,
        ICurrentUserService currentUserService)
    {
        _receptionistRepository = receptionistRepository;
        _officeRepository = officeRepository;
        _registeredEmailRepository = registeredEmailRepository;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<ReceptionistListItemDto>> GetListAsync()
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can view the receptionist list.");
        }

        var receptionists = await _receptionistRepository.GetAllAsync();
        var offices = await _officeRepository.GetAddressDictionaryAsync();

        return receptionists.Select(receptionist =>
        {
            offices.TryGetValue(receptionist.OfficeId, out var address);
            return receptionist.ToListItemDto(address);
        });
    }

    public async Task<ReceptionistResponseDto> GetByIdAsync(Guid id)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (!_currentUserService.IsReceptionistOrAdmin && _currentUserService.UserId != id)
        {
            throw new ForbiddenException("You can only view your own receptionist profile.");
        }

        var receptionist = await _receptionistRepository.GetByIdAsync(id);
        if (receptionist is null)
        {
            throw new NotFoundException($"Receptionist with ID '{id}' was not found.");
        }

        var office = await _officeRepository.GetByIdAsync(receptionist.OfficeId);
        return receptionist.ToResponseDto(office?.Address);
    }

    public async Task<ReceptionistResponseDto> CreateAsync(CreateReceptionistDto dto)
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can create receptionist profiles.");
        }

        ValidateReceptionist(dto.FirstName, dto.LastName, dto.Email, dto.OfficeId);
        await _officeRepository.EnsureExistsAndActiveAsync(dto.OfficeId);

        var emailExistsInReceptionists = await _receptionistRepository.EmailExistsAsync(dto.Email);
        var emailExistsInIdentity = await _registeredEmailRepository.ExistsAsync(dto.Email);
        if (emailExistsInReceptionists || emailExistsInIdentity)
        {
            throw new ConflictException("User with this email already exists.");
        }

        var entity = new ReceptionistEntity
        {
            PhotoUrl = dto.PhotoUrl,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            OfficeId = dto.OfficeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _receptionistRepository.AddAsync(entity);

        await _eventPublisher.PublishReceptionistProfileCreatedAsync(entity.Id, entity.Email, entity.FirstName, entity.LastName);

        var office = await _officeRepository.GetByIdAsync(entity.OfficeId);
        return entity.ToResponseDto(office?.Address);
    }

    public async Task<ReceptionistResponseDto> UpdateAsync(Guid id, UpdateReceptionistDto dto)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (!_currentUserService.IsReceptionistOrAdmin && _currentUserService.UserId != id)
        {
            throw new ForbiddenException("You can only edit your own receptionist profile.");
        }

        var receptionist = await _receptionistRepository.GetByIdAsync(id);
        if (receptionist is null)
        {
            throw new NotFoundException($"Receptionist with ID '{id}' was not found.");
        }

        PersonValidationHelper.ValidateName(dto.FirstName, dto.LastName);
        PersonValidationHelper.ValidateOfficeId(dto.OfficeId);

        await _officeRepository.EnsureExistsAndActiveAsync(dto.OfficeId);

        receptionist.PhotoUrl = dto.PhotoUrl;
        receptionist.FirstName = dto.FirstName.Trim();
        receptionist.LastName = dto.LastName.Trim();
        receptionist.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
        receptionist.OfficeId = dto.OfficeId;
        receptionist.UpdatedAt = DateTime.UtcNow;

        await _receptionistRepository.UpdateAsync(receptionist);

        var office = await _officeRepository.GetByIdAsync(receptionist.OfficeId);
        return receptionist.ToResponseDto(office?.Address);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!_currentUserService.IsReceptionistOrAdmin)
        {
            throw new ForbiddenException("Only receptionists or administrators can delete receptionist profiles.");
        }

        var receptionist = await _receptionistRepository.GetByIdAsync(id);
        if (receptionist is null)
        {
            throw new NotFoundException($"Receptionist with ID '{id}' was not found.");
        }

        await _receptionistRepository.DeleteAsync(id);
        await _eventPublisher.PublishReceptionistProfileDeletedAsync(id);
    }

    private static void ValidateReceptionist(string firstName, string lastName, string email, Guid officeId)
    {
        PersonValidationHelper.ValidateName(firstName, lastName);
        PersonValidationHelper.ValidateOfficeId(officeId);
        PersonValidationHelper.ValidateEmail(email);
    }
}
