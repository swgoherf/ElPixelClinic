using System.ComponentModel.DataAnnotations;
using Office.Domain.ValueObjects;

namespace Office.Domain.Entities;

public class OfficeEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Address Address { get; set; } = null!;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string RegistryPhoneNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
