using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Users;
using MaxVerse.API.Models.Order;

namespace MaxVerse.API.Models.Payment;

public class UserPaymentMethod : BaseEntity
{
    public int UserId { get; set; }

    public int PaymentTypeId { get; set; }

    public string? Provider { get; set; }

    public string? AccountNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool IsDefault { get; set; }

    //----------------Navigation----------------

    public User User { get; set; } = null!;

    public PaymentType PaymentType { get; set; } = null!;

    public ICollection<ShopOrder> Orders { get; set; }
        = new List<ShopOrder>();
}