using MaxVerse.API.Models.Base;

namespace MaxVerse.API.Models.Product;

public class ProductImage : BaseEntity
{
    public int ProductItemId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public ProductItem ProductItem { get; set; } = null!;
}