using System.ComponentModel;
using RouteXY.Api.Enums;

namespace RouteXY.Api.Requests;

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;

    [DefaultValue("@gmail.com")]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    [DefaultValue("+36201234567")]
    public string? Phone { get; set; }
}