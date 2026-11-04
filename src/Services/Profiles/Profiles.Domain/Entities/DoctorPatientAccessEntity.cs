namespace Profiles.Domain.Entities;

public class DoctorPatientAccessEntity
{
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
}
