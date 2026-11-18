namespace Profiles.Application.DTOs.PatientDtos;

public record PatientMatchCheckResultDto(
    bool HasMatch,
    Guid? ExistingProfileId,
    PatientPersonalInfoDto? MatchedProfile,
    PatientPersonalInfoDto? CreatedProfile
);
