using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Product;

namespace MaxVerse.API.Models.Order;

public class OrderLine : BaseEntity
{
    public int OrderId { get; set; }

    public int ProductItemId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? VariantDescription { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    //----------------Navigation----------------

    public ShopOrder Order { get; set; } = null!;

    public ProductItem ProductItem { get; set; } = null!;
}