using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Product;

public class Brand : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string BrandName { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}