namespace Profiles.Application.Common.Interfaces;

public interface IDoctorPatientAccessRepository
{
    Task<bool> HasAccessAsync(Guid doctorId, Guid patientId);
    Task GrantAccessAsync(Guid appointmentId, Guid doctorId, Guid patientId);
    Task RevokeAccessAsync(Guid appointmentId);
}
