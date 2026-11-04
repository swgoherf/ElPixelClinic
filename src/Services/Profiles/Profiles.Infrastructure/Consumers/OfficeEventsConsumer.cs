using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Application.Common.Events;
using Profiles.Application.Common.Interfaces;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Consumers;

public class OfficeEventsConsumer :
    IConsumer<OfficeCreatedOrUpdatedEvent>,
    IConsumer<OfficeDeletedEvent>
{
    private readonly IOfficeReplicaRepository _officeRepository;
    private readonly ILogger<OfficeEventsConsumer> _logger;

    public OfficeEventsConsumer(
        IOfficeReplicaRepository officeRepository,
        ILogger<OfficeEventsConsumer> logger)
    {
        _officeRepository = officeRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OfficeCreatedOrUpdatedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing OfficeCreatedOrUpdatedEvent for Office ID: {OfficeId}, Address: {Address}", msg.Id, msg.Address);

        await _officeRepository.UpsertAsync(new OfficeReplicaEntity
        {
            Id = msg.Id,
            Address = msg.Address,
            IsActive = msg.IsActive
        });
    }

    public async Task Consume(ConsumeContext<OfficeDeletedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing OfficeDeletedEvent for Office ID: {OfficeId}", msg.Id);

        await _officeRepository.DeleteAsync(msg.Id);
    }
}
