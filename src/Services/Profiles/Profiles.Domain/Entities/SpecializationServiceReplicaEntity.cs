namespace Profiles.Domain.Entities;

public class SpecializationServiceReplicaEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SpecializationName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}
