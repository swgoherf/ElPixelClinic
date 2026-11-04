using Services.Application.Common.Exceptions;
using Services.Application.Common.Interfaces;
using Services.Application.DTOs.MedicalServiceDtos;
using Services.Application.Mappings;
using Services.Domain.Entities;

namespace Services.Application.Services;

public class MedicalServiceService : IMedicalServiceService
{
    private readonly IMedicalServiceRepository _serviceRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IServicesEventPublisher _eventPublisher;

    public MedicalServiceService(
        IMedicalServiceRepository serviceRepository,
        ISpecializationRepository specializationRepository,
        IServicesEventPublisher eventPublisher)
    {
        _serviceRepository = serviceRepository;
        _specializationRepository = specializationRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<IEnumerable<MedicalServiceResponseDto>> GetAllAsync()
    {
        var services = await _serviceRepository.GetAllAsync();

        return services.Select(s => s.ToResponseDto());
    }

    public async Task<MedicalServiceResponseDto> GetByIdAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service is null)
        {
            throw new NotFoundException($"Medical service with ID '{id}' was not found.");
        }

        return service.ToResponseDto();
    }

    public async Task<MedicalServiceResponseDto> UpdateAsync(Guid id, UpdateMedicalServiceDto dto)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service is null)
        {
            throw new NotFoundException($"Medical service with ID '{id}' was not found.");
        }

        var oldSpecialization = await _specializationRepository.GetByIdAsync(service.SpecializationId);
        var oldServiceName = service.Name;

        var newSpecialization = await EnsureSpecializationExistsAsync(dto.SpecializationId);

        service.Name = dto.Name;
        service.Price = dto.Price;
        service.IsActive = dto.IsActive;
        service.SpecializationId = dto.SpecializationId;

        await _serviceRepository.UpdateAsync(service);

        if (oldSpecialization is not null && (oldSpecialization.Id != newSpecialization.Id || oldServiceName != service.Name))
        {
            await _eventPublisher.PublishSpecializationServiceDeletedAsync(oldSpecialization.Name, oldServiceName);
        }

        await _eventPublisher.PublishSpecializationServiceCreatedAsync(newSpecialization.Name, service.Name);

        return service.ToResponseDto();
    }

    public async Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto dto)
    {
        var specialization = await EnsureSpecializationExistsAsync(dto.SpecializationId);

        var entity = new MedicalServiceEntity
        {
            Name = dto.Name,
            Price = dto.Price,
            IsActive = dto.IsActive,
            SpecializationId = dto.SpecializationId,
        };

        await _serviceRepository.AddAsync(entity);

        await _eventPublisher.PublishSpecializationServiceCreatedAsync(specialization.Name, entity.Name);

        return entity.ToResponseDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service is null)
        {
            throw new NotFoundException($"Medical service with ID '{id}' was not found.");
        }

        var specialization = await _specializationRepository.GetByIdAsync(service.SpecializationId);

        await _serviceRepository.DeleteAsync(service);

        if (specialization is not null)
        {
            await _eventPublisher.PublishSpecializationServiceDeletedAsync(specialization.Name, service.Name);
        }
    }

    private async Task<SpecializationEntity> EnsureSpecializationExistsAsync(Guid specializationId)
    {
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);

        if (specialization is null)
        {
            throw new ValidationException($"Specialization with ID '{specializationId}' does not exist.");
        }

        return specialization;
    }
}
