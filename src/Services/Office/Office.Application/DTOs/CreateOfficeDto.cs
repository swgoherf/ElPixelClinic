using System.ComponentModel.DataAnnotations;

namespace Office.Application.DTOs;

public record CreateOfficeDto(
    [Required][MaxLength(100)] string City,
    [Required][MaxLength(150)] string Street,
    [Required][MaxLength(20)] string HouseNumber,
    [Required][Phone][MaxLength(20)] string RegistryPhoneNumber,
    bool IsActive = true
);
