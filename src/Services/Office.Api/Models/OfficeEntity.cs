using System.ComponentModel.DataAnnotations;

namespace Office.Api.Models;

public class OfficeEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string RegistryPhoneNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

