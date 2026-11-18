using Profiles.Application.Common.Helpers;
using Profiles.Application.DTOs.PatientDtos;
using Profiles.Domain.Entities;

namespace Profiles.Application.Mappings;

public static class PatientMappingExtensions
{
    public static string ToFullName(string firstName, string lastName, string? middleName) =>
        FullNameFormatter.Format(firstName, lastName, middleName);

    public static PatientPersonalInfoDto ToPersonalInfoDto(this PatientEntity entity)
    {
        return new PatientPersonalInfoDto(
            entity.Id,
            entity.PhotoUrl,
            entity.FirstName,
            entity.LastName,
            entity.MiddleName,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.PhoneNumber,
            entity.DateOfBirth,
            entity.IsLinkedToAccount
        );
    }

    public static PatientListItemDto ToListItemDto(this PatientEntity entity)
    {
        return new PatientListItemDto(
            entity.Id,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.PhoneNumber
        );
    }
}
