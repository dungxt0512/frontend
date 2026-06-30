using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Payment;

public class PaymentType : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    public ICollection<UserPaymentMethod> PaymentMethods { get; set; }
        = new List<UserPaymentMethod>();
}