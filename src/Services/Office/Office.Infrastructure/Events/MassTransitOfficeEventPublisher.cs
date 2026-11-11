using MassTransit;
using Microsoft.Extensions.Logging;
using ElPixelClinic.Contracts.Events;
using Office.Application.Common.Interfaces;

namespace Office.Infrastructure.Events;

public class MassTransitOfficeEventPublisher : IOfficeEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitOfficeEventPublisher> _logger;

    public MassTransitOfficeEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitOfficeEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishOfficeCreatedOrUpdatedAsync(Guid id, string address, bool isActive)
    {
        _logger.LogInformation("Publishing OfficeCreatedOrUpdatedEvent for Office ID: {OfficeId}, Address: {Address}", id, address);
        await _publishEndpoint.Publish(new OfficeCreatedOrUpdatedEvent(id, address, isActive));
    }

    public async Task PublishOfficeStatusChangedAsync(Guid id, bool isActive)
    {
        _logger.LogInformation("Publishing OfficeStatusChangedEvent for Office ID: {OfficeId}, IsActive: {IsActive}", id, isActive);
        await _publishEndpoint.Publish(new OfficeStatusChangedEvent(id, isActive));
    }

    public async Task PublishOfficeDeletedAsync(Guid id)
    {
        _logger.LogInformation("Publishing OfficeDeletedEvent for Office ID: {OfficeId}", id);
        await _publishEndpoint.Publish(new OfficeDeletedEvent(id));
    }
}
