namespace ElPixelClinic.Contracts.Events;

public record OfficeStatusChangedEvent(
    Guid OfficeId,
    bool IsActive
);
