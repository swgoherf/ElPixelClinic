namespace Profiles.Application.Common.Events;

public record SpecializationServiceDeletedEvent(
    string SpecializationName,
    string ServiceName
);
