namespace Office.Application.Common.Events;

public record OfficeCreatedOrUpdatedEvent(
    Guid Id,
    string Address,
    bool IsActive
);
