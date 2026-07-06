namespace RouteXY.Api.Modules.Auth.Requests;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public bool? IsActive { get; set; }
}