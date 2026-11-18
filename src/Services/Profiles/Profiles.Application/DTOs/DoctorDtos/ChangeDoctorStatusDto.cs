using Profiles.Domain.Enums;

namespace Profiles.Application.DTOs.DoctorDtos;

public record ChangeDoctorStatusDto(
    DoctorStatus Status
);
