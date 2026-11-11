using Profiles.Application.Common.Helpers;
using Profiles.Application.DTOs.ReceptionistDtos;
using Profiles.Domain.Entities;

namespace Profiles.Application.Mappings;

public static class ReceptionistMappingExtensions
{
    public static string ToFullName(string firstName, string lastName, string? middleName) =>
        FullNameFormatter.Format(firstName, lastName, middleName);

    public static ReceptionistResponseDto ToResponseDto(this ReceptionistEntity entity, string? officeAddress = null)
    {
        return new ReceptionistResponseDto(
            entity.Id,
            entity.PhotoUrl,
            entity.FirstName,
            entity.LastName,
            entity.MiddleName,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.Email,
            entity.OfficeId,
            officeAddress ?? $"Office {entity.OfficeId}"
        );
    }

    public static ReceptionistListItemDto ToListItemDto(this ReceptionistEntity entity, string? officeAddress = null)
    {
        return new ReceptionistListItemDto(
            entity.Id,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.OfficeId,
            officeAddress ?? $"Office {entity.OfficeId}"
        );
    }
}
