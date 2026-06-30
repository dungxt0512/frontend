using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Product;

public class Variation : BaseEntity
{
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ProductCategory Category { get; set; } = null!;

    public ICollection<VariationOption> Options { get; set; }
        = new List<VariationOption>();
}