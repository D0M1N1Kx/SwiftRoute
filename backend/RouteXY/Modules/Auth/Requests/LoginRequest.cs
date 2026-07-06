using System.ComponentModel;

namespace RouteXY.Api.Modules.Auth.Requests;

public class LoginRequest
{
    [DefaultValue("@gmail.com")]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}