namespace Services.Application.Common.Events;

public record SpecializationServiceCreatedEvent(
    string SpecializationName,
    string ServiceName
);
