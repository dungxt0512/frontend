using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Product;

namespace MaxVerse.API.Models.Order;

public class ShoppingCartItem : BaseEntity
{
    public int CartId { get; set; }

    public int ProductItemId { get; set; }

    public int Quantity { get; set; }

    public ShoppingCart Cart { get; set; } = null!;

    public ProductItem ProductItem { get; set; } = null!;
}