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

    public async Task<MedicalServiceResponseDto?> GetByIdAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        return service?.ToResponseDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMedicalServiceDto dto)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service is null)
        {
            return false;
        }

        var specializationExists = await _specializationRepository.ExistsAsync(dto.SpecializationId);

        if (!specializationExists)
        {
            throw new ArgumentException($"Specialization with ID '{dto.SpecializationId}' does not exist.");
        }

        service.Name = dto.Name;
        service.Price = dto.Price;
        service.IsActive = dto.IsActive;
        service.SpecializationId = dto.SpecializationId;

        await _serviceRepository.UpdateAsync(service);

        return true;
    }

    public async Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto dto)
    {
        var specializationExists = await _specializationRepository.ExistsAsync(dto.SpecializationId);

        if (!specializationExists)
        {
            throw new ArgumentException($"Specialization with ID '{dto.SpecializationId}' does not exist.");
        }

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

    public async Task<bool> DeleteAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service is null)
        {
            return false;
        }

        await _serviceRepository.DeleteAsync(service);

        return true;
    }
}
