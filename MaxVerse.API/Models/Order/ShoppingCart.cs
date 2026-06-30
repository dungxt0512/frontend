using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Users;

namespace MaxVerse.API.Models.Order;

public class ShoppingCart : BaseEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<ShoppingCartItem> Items { get; set; }
        = new List<ShoppingCartItem>();
}