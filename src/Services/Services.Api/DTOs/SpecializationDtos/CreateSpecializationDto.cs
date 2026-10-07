using System.ComponentModel.DataAnnotations;

namespace Services.Api.DTOs.SpecializationDtos;

public record CreateSpecializationDto(
    [Required][MaxLength(100)] string Name
);

