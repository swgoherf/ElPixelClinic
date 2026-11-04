using Profiles.Application.Common.Helpers;
using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Domain.Entities;

namespace Profiles.Application.Mappings;

public static class DoctorMappingExtensions
{
    public static string ToFullName(string firstName, string lastName, string? middleName) =>
        FullNameFormatter.Format(firstName, lastName, middleName);

    public static int CalculateExperience(int careerStartYear)
    {
        var currentYear = DateTime.UtcNow.Year;
        return careerStartYear <= currentYear ? currentYear - careerStartYear + 1 : 0;
    }

    public static DoctorCardResponseDto ToCardResponseDto(this DoctorEntity entity, string? officeAddress = null)
    {
        return new DoctorCardResponseDto(
            entity.Id,
            entity.PhotoUrl,
            entity.FirstName,
            entity.LastName,
            entity.MiddleName,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.Specialization,
            CalculateExperience(entity.CareerStartYear),
            entity.OfficeId,
            officeAddress ?? $"Office {entity.OfficeId}"
        );
    }

    public static DoctorDetailResponseDto ToDetailResponseDto(this DoctorEntity entity, string? officeAddress = null, IReadOnlyList<string>? services = null)
    {
        return new DoctorDetailResponseDto(
            entity.Id,
            entity.PhotoUrl,
            entity.FirstName,
            entity.LastName,
            entity.MiddleName,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.Specialization,
            CalculateExperience(entity.CareerStartYear),
            entity.OfficeId,
            officeAddress ?? $"Office {entity.OfficeId}",
            services ?? Array.Empty<string>()
        );
    }

    public static DoctorAdminResponseDto ToAdminResponseDto(this DoctorEntity entity, string? officeAddress = null)
    {
        return new DoctorAdminResponseDto(
            entity.Id,
            entity.PhotoUrl,
            entity.FirstName,
            entity.LastName,
            entity.MiddleName,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.DateOfBirth,
            entity.Email,
            entity.Specialization,
            entity.OfficeId,
            officeAddress ?? $"Office {entity.OfficeId}",
            entity.CareerStartYear,
            CalculateExperience(entity.CareerStartYear),
            entity.Status
        );
    }

    public static DoctorOwnerResponseDto ToOwnerResponseDto(this DoctorEntity entity, string? officeAddress = null)
    {
        return new DoctorOwnerResponseDto(
            entity.Id,
            entity.PhotoUrl,
            entity.FirstName,
            entity.LastName,
            entity.MiddleName,
            ToFullName(entity.FirstName, entity.LastName, entity.MiddleName),
            entity.DateOfBirth,
            entity.Email,
            entity.Specialization,
            entity.OfficeId,
            officeAddress ?? $"Office {entity.OfficeId}",
            entity.CareerStartYear,
            CalculateExperience(entity.CareerStartYear)
        );
    }
}
