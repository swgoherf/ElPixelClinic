using ElPixelClinic.Contracts.Events;

namespace Office.Application.Common.Interfaces;

public interface IOfficeEventPublisher
{
    Task PublishOfficeCreatedOrUpdatedAsync(Guid id, string address, bool isActive);
    Task PublishOfficeStatusChangedAsync(Guid id, bool isActive);
    Task PublishOfficeDeletedAsync(Guid id);
}
