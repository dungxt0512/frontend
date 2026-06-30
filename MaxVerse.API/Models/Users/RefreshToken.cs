using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MaxVerse.API.Models.Users;

public class RefreshToken : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked { get; set; }

    public int UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}