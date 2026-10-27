using Services.Application.Common.Exceptions;
using Services.Application.Common.Interfaces;
using Services.Application.DTOs.SpecializationDtos;
using Services.Application.Mappings;
using Services.Domain.Entities;

namespace Services.Application.Services;

public class SpecializationService : ISpecializationService
{
    private readonly ISpecializationRepository _specializationRepository;

    public SpecializationService(ISpecializationRepository specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    public async Task<IEnumerable<SpecializationResponseDto>> GetAllAsync()
    {
        var specializations = await _specializationRepository.GetAllAsync();

        return specializations.Select(s => s.ToResponseDto());
    }

    public async Task<SpecializationResponseDto> GetByIdAsync(Guid id)
    {
        var specialization = await _specializationRepository.GetByIdAsync(id);

        if (specialization is null)
        {
            throw new NotFoundException($"Specialization with ID '{id}' was not found.");
        }

        return specialization.ToResponseDto();
    }

    public async Task<SpecializationResponseDto> UpdateAsync(Guid id, UpdateSpecializationDto dto)
    {
        var specialization = await _specializationRepository.GetByIdAsync(id);

        if (specialization is null)
        {
            throw new NotFoundException($"Specialization with ID '{id}' was not found.");
        }

        specialization.Name = dto.Name;

        await _specializationRepository.UpdateAsync(specialization);

        return specialization.ToResponseDto();
    }

    public async Task<SpecializationResponseDto> CreateAsync(CreateSpecializationDto dto)
    {
        var entity = new SpecializationEntity
        {
            Name = dto.Name
        };

        await _specializationRepository.AddAsync(entity);

        return entity.ToResponseDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var specialization = await _specializationRepository.GetByIdAsync(id);

        if (specialization is null)
        {
            throw new NotFoundException($"Specialization with ID '{id}' was not found.");
        }

        await _specializationRepository.DeleteAsync(specialization);
    }
}
