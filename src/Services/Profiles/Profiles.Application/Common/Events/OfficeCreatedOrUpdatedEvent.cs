namespace Profiles.Application.Common.Events;

public record OfficeCreatedOrUpdatedEvent(
    Guid Id,
    string Address,
    bool IsActive
);
