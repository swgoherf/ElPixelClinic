using System.ComponentModel.DataAnnotations;

namespace Services.Api.Models;

public class SpecializationEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ServiceEntity> Services { get; set; } = new List<ServiceEntity>();
}

