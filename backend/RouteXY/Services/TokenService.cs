using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RouteXY.Api.Class;
using RouteXY.Api.Entities;
using RouteXY.Api.Enums;
using RouteXY.Api.Settings;

namespace RouteXY.Api.Services;

public class TokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(ClaimTypes.Role, user.Role.ToString()),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    public UserClaims? GetUserClaimsFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Bearer ".Length..].Trim();
        }

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                          
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value 
                             ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
                             
            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value 
                            ?? principal.FindFirst("role")?.Value;

            if (idClaim == null || emailClaim == null || roleClaim == null)
                return null;
            
            if (!Guid.TryParse(idClaim, out var userId))
                return null;

            if (!Enum.TryParse<UserRole>(roleClaim, out var userRole))
                return null;

            return new UserClaims
            {
                Id = userId,
                Email = emailClaim,
                Role = userRole
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}