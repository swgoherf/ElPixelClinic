namespace ElPixelClinic.Contracts.Events;

public record SpecializationServiceCreatedEvent(
    string SpecializationName,
    string ServiceName
);
