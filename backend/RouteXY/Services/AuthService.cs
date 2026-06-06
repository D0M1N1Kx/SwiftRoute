using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;

namespace RouteXY.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> RegisterAsync(CreateUserRequest request)
    {
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Email already in use");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            Phone = request.Phone
        };

        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(refreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _db.Users.Add(user);
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        var accessToken = _tokenService.GenerateAccessToken(user);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            User = new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }
}