using ElPixelClinic.Contracts.Events;

namespace Services.Application.Common.Interfaces;

public interface IServicesEventPublisher
{
    Task PublishSpecializationServiceCreatedAsync(string specializationName, string serviceName);
    Task PublishSpecializationServiceDeletedAsync(string specializationName, string serviceName);
}
