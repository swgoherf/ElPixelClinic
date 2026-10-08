using System.ComponentModel.DataAnnotations;

namespace Services.Api.DTOs.SpecializationDtos;

public record UpdateSpecializationDto(
    [Required][MaxLength(100)] string Name
);
    

