using MaxVerse.API.Models.Base;

namespace MaxVerse.API.Models.Order;

public class OrderStatus : BaseEntity
{
    public string Status { get; set; } = string.Empty;

    public ICollection<ShopOrder> Orders { get; set; }
        = new List<ShopOrder>();
}