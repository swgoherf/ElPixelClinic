namespace Services.Api.DTOs.ServiceDtos;

public record ServiceResponseDto(
    Guid Id,
    string Name,
    decimal Price,
    bool IsActive,
    Guid SpecializationId
);

