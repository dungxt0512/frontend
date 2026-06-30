using MaxVerse.API.DTOs.Auth;

namespace MaxVerse.API.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<AuthResponse> RefreshTokenAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);
}