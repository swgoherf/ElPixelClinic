namespace Profiles.Application.Common.Events;

public record SpecializationServiceCreatedEvent(
    string SpecializationName,
    string ServiceName
);
