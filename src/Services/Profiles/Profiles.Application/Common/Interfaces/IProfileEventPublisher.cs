namespace Profiles.Application.Common.Interfaces;

public interface IProfileEventPublisher
{
    Task PublishDoctorProfileCreatedAsync(Guid doctorId, string email, string firstName, string lastName);
    Task PublishReceptionistProfileCreatedAsync(Guid receptionistId, string email, string firstName, string lastName);
    Task PublishPatientProfileDeletedAsync(Guid patientId);
    Task PublishReceptionistProfileDeletedAsync(Guid receptionistId);
}
