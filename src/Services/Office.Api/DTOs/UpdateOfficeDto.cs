using System.ComponentModel.DataAnnotations;

namespace Office.Api.DTOs;

public record UpdateOfficeDto(
    [Required][MaxLength(250)] string Address,
    [Required][Phone][MaxLength(20)] string RegistryPhoneNumber,
    bool IsActive = true
);
