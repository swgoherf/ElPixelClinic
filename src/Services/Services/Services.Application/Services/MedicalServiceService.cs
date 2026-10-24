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

    public MedicalServiceService(
        IMedicalServiceRepository serviceRepository,
        ISpecializationRepository specializationRepository)
    {
        _serviceRepository = serviceRepository;
        _specializationRepository = specializationRepository;
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

        await EnsureSpecializationExistsAsync(dto.SpecializationId);

        service.Name = dto.Name;
        service.Price = dto.Price;
        service.IsActive = dto.IsActive;
        service.SpecializationId = dto.SpecializationId;

        await _serviceRepository.UpdateAsync(service);

        return service.ToResponseDto();
    }

    public async Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto dto)
    {
        await EnsureSpecializationExistsAsync(dto.SpecializationId);

        var entity = new MedicalServiceEntity
        {
            Name = dto.Name,
            Price = dto.Price,
            IsActive = dto.IsActive,
            SpecializationId = dto.SpecializationId,
        };

        await _serviceRepository.AddAsync(entity);

        return entity.ToResponseDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service is null)
        {
            throw new NotFoundException($"Medical service with ID '{id}' was not found.");
        }

        await _serviceRepository.DeleteAsync(service);
    }

    private async Task EnsureSpecializationExistsAsync(Guid specializationId)
    {
        var exists = await _specializationRepository.ExistsAsync(specializationId);

        if (!exists)
        {
            throw new ValidationException($"Specialization with ID '{specializationId}' does not exist.");
        }
    }
}
