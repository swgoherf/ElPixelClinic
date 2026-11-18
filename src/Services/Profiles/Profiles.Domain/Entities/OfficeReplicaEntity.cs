namespace Profiles.Domain.Entities;

public class OfficeReplicaEntity
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
