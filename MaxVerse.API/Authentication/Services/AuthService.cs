using MaxVerse.API.Authentication.Interfaces;
using MaxVerse.API.Data;
using MaxVerse.API.DTOs.Auth;
using MaxVerse.API.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace MaxVerse.API.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly MaxVerseDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(
        MaxVerseDbContext context,
        IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(x => x.Email == request.Email))
            throw new Exception("Email already exists.");

        var customerRole = await _context.Roles
            .FirstAsync(x => x.Name == "Customer");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = customerRole.Id
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        user.Role = customerRole;

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _jwtService.GenerateRefreshToken();

        var refresh = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refresh);

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = customerRole.Name,

            AccessToken = accessToken,

            RefreshToken = refreshToken,

            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
        .Include(x => x.Role)
        .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
            throw new Exception("Email hoặc mật khẩu không đúng.");

        if (!user.IsActive)
            throw new Exception("Tài khoản đã bị khóa.");

        var validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!validPassword)
            throw new Exception("Email hoặc mật khẩu không đúng.");

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _jwtService.GenerateRefreshToken();

        var refresh = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            RevokedAt = null
        };

        _context.RefreshTokens.Add(refresh);

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name,

            AccessToken = accessToken,
            RefreshToken = refreshToken,

            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
        .Include(x => x.User)
            .ThenInclude(x => x.Role)
        .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token == null)
            throw new Exception("Refresh Token không tồn tại.");

        if (token.IsRevoked)
            throw new Exception("Refresh Token đã bị thu hồi.");

        if (token.ExpiresAt <= DateTime.UtcNow)
            throw new Exception("Refresh Token đã hết hạn.");

        var accessToken = _jwtService.GenerateAccessToken(token.User);

        return new AuthResponse
        {
            UserId = token.User.Id,
            FullName = token.User.FullName,
            Email = token.User.Email,
            Role = token.User.Role.Name,

            AccessToken = accessToken,

            RefreshToken = token.Token,

            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
        .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token == null)
            return;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}