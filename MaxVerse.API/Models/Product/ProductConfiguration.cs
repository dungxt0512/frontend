namespace MaxVerse.API.Models.Product;

public class ProductConfiguration
{
    public int ProductItemId { get; set; }

    public int VariationOptionId { get; set; }

    public ProductItem ProductItem { get; set; } = null!;

    public VariationOption VariationOption { get; set; } = null!;
}