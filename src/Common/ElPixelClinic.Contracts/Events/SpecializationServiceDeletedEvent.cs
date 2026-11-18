namespace ElPixelClinic.Contracts.Events;

public record SpecializationServiceDeletedEvent(
    string SpecializationName,
    string ServiceName
);
