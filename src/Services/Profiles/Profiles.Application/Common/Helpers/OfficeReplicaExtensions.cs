using Profiles.Application.Common.Exceptions;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Application.Common.Helpers;

public static class OfficeReplicaExtensions
{
    public static async Task<Dictionary<Guid, string>> GetAddressDictionaryAsync(this IOfficeReplicaRepository officeRepository)
    {
        var offices = await officeRepository.GetAllAsync();

        return offices.ToDictionary(x => x.Id, x => x.Address);
    }

    public static async Task EnsureExistsAndActiveAsync(this IOfficeReplicaRepository officeRepository, Guid officeId)
    {
        var office = await officeRepository.GetByIdAsync(officeId);

        if (office is null)
        {
            throw new ValidationException($"Office with ID '{officeId}' does not exist.");
        }

        if (!office.IsActive)
        {
            throw new ValidationException($"Office with ID '{officeId}' is inactive.");
        }
    }
}
