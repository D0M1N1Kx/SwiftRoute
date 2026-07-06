using RouteXY.Api.Auth.Responses;

namespace RouteXY.Api.Modules.Auth.Responses;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserResponse User { get; set; } = null!;
}