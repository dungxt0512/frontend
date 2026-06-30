using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Product;

public class Product : BaseEntity
{
    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ThumbnailUrl { get; set; }

    public bool IsActive { get; set; } = true;

    //----------------Navigation----------------

    public Brand Brand { get; set; } = null!;

    public ProductCategory Category { get; set; } = null!;

    public ICollection<ProductItem> ProductItems { get; set; }
        = new List<ProductItem>();
}