using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Common;
using MaxVerse.API.Models.Order;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Users;

public class Address : BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string RecipientPhone { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StreetAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Ward { get; set; }

    [Required]
    [MaxLength(100)]
    public string District { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    public int CountryId { get; set; }

    //----------------Navigation----------------

    public Country Country { get; set; } = null!;

    public ICollection<UserAddress> UserAddresses { get; set; }
        = new List<UserAddress>();

    public ICollection<ShopOrder> Orders { get; set; }
        = new List<ShopOrder>();
}