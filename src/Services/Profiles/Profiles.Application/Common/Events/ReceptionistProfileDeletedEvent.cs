namespace Profiles.Application.Common.Events;

public record ReceptionistProfileDeletedEvent(
    Guid ReceptionistId
);
