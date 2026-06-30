using MaxVerse.API.Models.Users;

namespace MaxVerse.API.Authentication.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();
}