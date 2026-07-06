using RouteXY.Api.Enums;

namespace RouteXY.Api.Class;

public class UserClaims
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}