using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Application.Common.Events;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Consumers;

public class UserEmailEventsConsumer :
    IConsumer<UserEmailRegisteredEvent>,
    IConsumer<UserEmailUnregisteredEvent>
{
    private readonly IRegisteredEmailReplicaRepository _registeredEmailRepository;
    private readonly ILogger<UserEmailEventsConsumer> _logger;

    public UserEmailEventsConsumer(
        IRegisteredEmailReplicaRepository registeredEmailRepository,
        ILogger<UserEmailEventsConsumer> logger)
    {
        _registeredEmailRepository = registeredEmailRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserEmailRegisteredEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing UserEmailRegisteredEvent for User ID: {UserId}, Email: {Email}", msg.UserId, msg.Email);

        await _registeredEmailRepository.UpsertAsync(msg.Email, msg.UserId);
    }

    public async Task Consume(ConsumeContext<UserEmailUnregisteredEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing UserEmailUnregisteredEvent for User ID: {UserId}, Email: {Email}", msg.UserId, msg.Email);

        await _registeredEmailRepository.DeleteAsync(msg.Email);
    }
}
