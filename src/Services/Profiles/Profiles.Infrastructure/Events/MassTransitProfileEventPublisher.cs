using MassTransit;
using Microsoft.Extensions.Logging;
using ElPixelClinic.Contracts.Events;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Events;

public class MassTransitProfileEventPublisher : IProfileEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitProfileEventPublisher> _logger;

    public MassTransitProfileEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitProfileEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishDoctorProfileCreatedAsync(Guid doctorId, string email, string firstName, string lastName)
    {
        _logger.LogInformation("Publishing DoctorProfileCreatedEvent for DoctorId: {DoctorId}, Email: {Email}", doctorId, email);

        await _publishEndpoint.Publish(new DoctorProfileCreatedEvent(doctorId, email, firstName, lastName));
    }

    public async Task PublishReceptionistProfileCreatedAsync(Guid receptionistId, string email, string firstName, string lastName)
    {
        _logger.LogInformation("Publishing ReceptionistProfileCreatedEvent for ReceptionistId: {ReceptionistId}, Email: {Email}", receptionistId, email);

        await _publishEndpoint.Publish(new ReceptionistProfileCreatedEvent(receptionistId, email, firstName, lastName));
    }

    public async Task PublishPatientProfileDeletedAsync(Guid patientId)
    {
        _logger.LogInformation("Publishing PatientProfileDeletedEvent for PatientId: {PatientId}", patientId);

        await _publishEndpoint.Publish(new PatientProfileDeletedEvent(patientId));
    }

    public async Task PublishReceptionistProfileDeletedAsync(Guid receptionistId)
    {
        _logger.LogInformation("Publishing ReceptionistProfileDeletedEvent for ReceptionistId: {ReceptionistId}", receptionistId);

        await _publishEndpoint.Publish(new ReceptionistProfileDeletedEvent(receptionistId));
    }
}
