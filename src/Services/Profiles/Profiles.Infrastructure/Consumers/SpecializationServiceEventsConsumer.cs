using MassTransit;
using Microsoft.Extensions.Logging;
using ElPixelClinic.Contracts.Events;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Consumers;

public class SpecializationServiceEventsConsumer :
    IConsumer<SpecializationServiceCreatedEvent>,
    IConsumer<SpecializationServiceDeletedEvent>
{
    private readonly ISpecializationServiceReplicaRepository _specializationServiceRepository;
    private readonly ILogger<SpecializationServiceEventsConsumer> _logger;

    public SpecializationServiceEventsConsumer(
        ISpecializationServiceReplicaRepository specializationServiceRepository,
        ILogger<SpecializationServiceEventsConsumer> logger)
    {
        _specializationServiceRepository = specializationServiceRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SpecializationServiceCreatedEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Processing SpecializationServiceCreatedEvent for Specialization: {Specialization}, Service: {Service}",
            msg.SpecializationName,
            msg.ServiceName
        );

        await _specializationServiceRepository.AddAsync(msg.SpecializationName, msg.ServiceName);
    }

    public async Task Consume(ConsumeContext<SpecializationServiceDeletedEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Processing SpecializationServiceDeletedEvent for Specialization: {Specialization}, Service: {Service}",
            msg.SpecializationName,
            msg.ServiceName
        );

        await _specializationServiceRepository.DeleteAsync(msg.SpecializationName, msg.ServiceName);
    }
}
