namespace Services.Application.Common.Events;

public record SpecializationServiceDeletedEvent(
    string SpecializationName,
    string ServiceName
);
