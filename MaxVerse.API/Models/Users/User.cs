using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Order;
using MaxVerse.API.Models.Payment;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Users;

public class User : BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }

    //----------------Navigation----------------
    public Role Role { get; set; } = null!;

    public ICollection<UserAddress> UserAddresses { get; set; }
        = new List<UserAddress>();

    public ICollection<UserPaymentMethod> PaymentMethods { get; set; }
        = new List<UserPaymentMethod>();

    public ShoppingCart? ShoppingCart { get; set; }

    public ICollection<ShopOrder> Orders { get; set; }
        = new List<ShopOrder>();

    public ICollection<UserReview> Reviews { get; set; }
        = new List<UserReview>();
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();
}