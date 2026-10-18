using System.ComponentModel.DataAnnotations;

namespace Services.Application.DTOs.ServiceDtos;

public record CreateServiceDto(
    [Required][MaxLength(150)] string Name,
    [Range(0, 1000000)] decimal Price,
    bool IsActive = true,
    [Required] Guid SpecializationId = default
);
