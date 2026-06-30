using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Payment;
using MaxVerse.API.Models.Users;

namespace MaxVerse.API.Models.Order;

public class ShopOrder : BaseEntity
{
    public string OrderCode { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int ShippingAddressId { get; set; }

    public int PaymentMethodId { get; set; }

    public int ShippingMethodId { get; set; }

    public int OrderStatusId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Note { get; set; }

    //----------------Navigation----------------

    public User User { get; set; } = null!;

    public Address ShippingAddress { get; set; } = null!;

    public UserPaymentMethod PaymentMethod { get; set; } = null!;

    public ShippingMethod ShippingMethod { get; set; } = null!;

    public OrderStatus OrderStatus { get; set; } = null!;

    public ICollection<OrderLine> OrderLines { get; set; }
        = new List<OrderLine>();
}