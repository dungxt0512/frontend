using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Product;

public class ProductItem : BaseEntity
{
    public int ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SKU { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal? SalePrice { get; set; }

    public int QtyInStock { get; set; }

    public string? Barcode { get; set; }

    public decimal? Weight { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();

    public ICollection<ProductConfiguration> Configurations { get; set; }
        = new List<ProductConfiguration>();
}