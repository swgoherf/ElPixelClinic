using System.ComponentModel.DataAnnotations;

namespace Services.Application.DTOs.SpecializationDtos;

public record UpdateSpecializationDto(
    [Required][MaxLength(100)] string Name
);
