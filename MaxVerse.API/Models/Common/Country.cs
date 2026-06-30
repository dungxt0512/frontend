using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;
using MaxVerse.API.Models.Users;

namespace MaxVerse.API.Models.Common;

public class Country : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string CountryName { get; set; } = string.Empty;

    public ICollection<Address> Addresses { get; set; }
        = new List<Address>();
}