using Office.Application.Common.Exceptions;
using Office.Application.Common.Interfaces;
using Office.Application.DTOs;
using Office.Application.Mappings;
using Office.Domain.Entities;

namespace Office.Application.Services;

public class OfficeService : IOfficeService
{
    private readonly IOfficeRepository _officeRepository;

    public OfficeService(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<IEnumerable<OfficeResponseDto>> GetAllAsync()
    {
        var offices = await _officeRepository.GetAllAsync();

        return offices.Select(o => o.ToResponseDto());
    }

    public async Task<OfficeResponseDto> GetByIdAsync(Guid id)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        if (office is null)
        {
            throw new NotFoundException($"Office with ID '{id}' was not found.");
        }

        return office.ToResponseDto();
    }

    public async Task<OfficeResponseDto> UpdateAsync(Guid id, UpdateOfficeDto dto)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        if (office is null)
        {
            throw new NotFoundException($"Office with ID '{id}' was not found.");
        }

        office.Address = dto.Address;
        office.RegistryPhoneNumber = dto.RegistryPhoneNumber;
        office.IsActive = dto.IsActive;

        await _officeRepository.UpdateAsync(office);

        return office.ToResponseDto();
    }

    public async Task<OfficeResponseDto> CreateAsync(CreateOfficeDto dto)
    {
        var entity = new OfficeEntity
        {
            Address = dto.Address,
            RegistryPhoneNumber = dto.RegistryPhoneNumber,
            IsActive = dto.IsActive
        };

        await _officeRepository.AddAsync(entity);

        return entity.ToResponseDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        if (office is null)
        {
            throw new NotFoundException($"Office with ID '{id}' was not found.");
        }

        await _officeRepository.DeleteAsync(office);
    }
}
