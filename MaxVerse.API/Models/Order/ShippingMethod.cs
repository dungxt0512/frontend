using MaxVerse.API.Models.Base;

namespace MaxVerse.API.Models.Order;

public class ShippingMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public ICollection<ShopOrder> Orders { get; set; }
        = new List<ShopOrder>();
}