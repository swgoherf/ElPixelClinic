using System.ComponentModel.DataAnnotations;

namespace Services.Application.DTOs.SpecializationDtos;

public record CreateSpecializationDto(
    [Required][MaxLength(100)] string Name
);
