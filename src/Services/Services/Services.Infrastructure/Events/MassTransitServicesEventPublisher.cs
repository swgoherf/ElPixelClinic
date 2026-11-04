using MassTransit;
using Microsoft.Extensions.Logging;
using Services.Application.Common.Events;
using Services.Application.Common.Interfaces;

namespace Services.Infrastructure.Events;

public class MassTransitServicesEventPublisher : IServicesEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitServicesEventPublisher> _logger;

    public MassTransitServicesEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitServicesEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishSpecializationServiceCreatedAsync(string specializationName, string serviceName)
    {
        _logger.LogInformation("Publishing SpecializationServiceCreatedEvent for Specialization: {Specialization}, Service: {Service}",
            specializationName, serviceName);
        await _publishEndpoint.Publish(new SpecializationServiceCreatedEvent(specializationName, serviceName));
    }

    public async Task PublishSpecializationServiceDeletedAsync(string specializationName, string serviceName)
    {
        _logger.LogInformation("Publishing SpecializationServiceDeletedEvent for Specialization: {Specialization}, Service: {Service}",
            specializationName, serviceName);
        await _publishEndpoint.Publish(new SpecializationServiceDeletedEvent(specializationName, serviceName));
    }
}
