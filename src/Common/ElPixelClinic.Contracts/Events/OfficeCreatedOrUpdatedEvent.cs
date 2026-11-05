namespace ElPixelClinic.Contracts.Events;

public record OfficeCreatedOrUpdatedEvent(
    Guid Id,
    string Address,
    bool IsActive
);
