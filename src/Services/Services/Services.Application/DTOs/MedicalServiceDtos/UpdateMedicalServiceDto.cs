using System.ComponentModel.DataAnnotations;

namespace Services.Application.DTOs.MedicalServiceDtos;

public record UpdateMedicalServiceDto(
    [Required][MaxLength(150)] string Name,
    [Range(0, 1000000)] decimal Price,
    bool IsActive = true,
    [Required] Guid SpecializationId = default
);
