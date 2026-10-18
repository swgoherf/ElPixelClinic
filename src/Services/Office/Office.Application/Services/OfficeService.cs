using Office.Application.Common.Interfaces;
using Office.Application.DTOs;
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

        return offices.Select(o => new OfficeResponseDto(
            o.Id,
            o.Address,
            o.RegistryPhoneNumber,
            o.IsActive
        ));
    }

    public async Task<OfficeResponseDto?> GetByIdAsync(Guid id)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        if (office is null)
        {
            return null;
        }

        return new OfficeResponseDto(
            office.Id,
            office.Address,
            office.RegistryPhoneNumber,
            office.IsActive
        );
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateOfficeDto dto)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        if (office is null)
        {
            return false;
        }

        office.Address = dto.Address;
        office.RegistryPhoneNumber = dto.RegistryPhoneNumber;
        office.IsActive = dto.IsActive;

        await _officeRepository.UpdateAsync(office);

        return true;
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

        var response = new OfficeResponseDto(
            entity.Id,
            entity.Address,
            entity.RegistryPhoneNumber,
            entity.IsActive
        );

        return response;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var office = await _officeRepository.GetByIdAsync(id);

        if (office is null)
        {
            return false;
        }

        await _officeRepository.DeleteAsync(office);

        return true;
    }
}
