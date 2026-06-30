using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Product;

public class VariationOption : BaseEntity
{
    public int VariationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    public Variation Variation { get; set; } = null!;

    public ICollection<ProductConfiguration> ProductConfigurations { get; set; }
        = new List<ProductConfiguration>();
}