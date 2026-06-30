using MaxVerse.API.Models.Base;
using MaxVerse.API.Models.Product;
using MaxVerse.API.Models.Users;

namespace MaxVerse.API.Models.Order;

public class UserReview : BaseEntity
{
    public int UserId { get; set; }

    public int ProductItemId { get; set; }

    public int RatingValue { get; set; }

    public string? Title { get; set; }

    public string? Comment { get; set; }

    //----------------Navigation----------------

    public User User { get; set; } = null!;

    public ProductItem ProductItem { get; set; } = null!;
}