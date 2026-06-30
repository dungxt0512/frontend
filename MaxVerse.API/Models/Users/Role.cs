using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Users;

public class Role : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; }
        = new List<User>();
}