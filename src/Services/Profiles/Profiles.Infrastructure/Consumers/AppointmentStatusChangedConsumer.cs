using MassTransit;
using Microsoft.Extensions.Logging;
using ElPixelClinic.Contracts.Events;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Consumers;

public class AppointmentStatusChangedConsumer : IConsumer<AppointmentStatusChangedEvent>
{
    private readonly IDoctorPatientAccessRepository _accessRepository;
    private readonly ILogger<AppointmentStatusChangedConsumer> _logger;

    public AppointmentStatusChangedConsumer(
        IDoctorPatientAccessRepository accessRepository,
        ILogger<AppointmentStatusChangedConsumer> logger)
    {
        _accessRepository = accessRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AppointmentStatusChangedEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Processing AppointmentStatusChangedEvent for Appointment ID: {AppointmentId}, Doctor: {DoctorId}, Patient: {PatientId}, Status: {Status}",
            msg.AppointmentId,
            msg.DoctorId,
            msg.PatientId,
            msg.Status
        );

        if (string.Equals(msg.Status, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            await _accessRepository.GrantAccessAsync(msg.AppointmentId, msg.DoctorId, msg.PatientId);
        }
        else
        {
            await _accessRepository.RevokeAccessAsync(msg.AppointmentId);
        }
    }
}
