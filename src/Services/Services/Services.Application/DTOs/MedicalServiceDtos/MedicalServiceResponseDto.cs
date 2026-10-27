namespace Services.Application.DTOs.MedicalServiceDtos;

public record MedicalServiceResponseDto(
    Guid Id,
    string Name,
    decimal Price,
    bool IsActive,
    Guid SpecializationId
);
