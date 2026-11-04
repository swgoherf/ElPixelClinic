using Office.Application.Common.Events;

namespace Office.Application.Common.Interfaces;

public interface IOfficeEventPublisher
{
    Task PublishOfficeCreatedOrUpdatedAsync(Guid id, string address, bool isActive);
    Task PublishOfficeDeletedAsync(Guid id);
}
