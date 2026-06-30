namespace MaxVerse.API.Models.Users;

public class UserAddress
{
    public int UserId { get; set; }

    public int AddressId { get; set; }

    public bool IsDefault { get; set; }

    //----------------Navigation----------------

    public User User { get; set; } = null!;

    public Address Address { get; set; } = null!;
}