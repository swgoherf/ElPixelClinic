using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Api.Models;

public class ServiceEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 1000000)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public Guid SpecializationId { get; set; }

    [ForeignKey(nameof(SpecializationId))]
    public SpecializationEntity? Specialization { get; set; }
}

