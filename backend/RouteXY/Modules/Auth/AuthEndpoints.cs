using FluentValidation;
using RouteXY.Api.Modules.Auth.Requests;

namespace RouteXY.Api.Modules.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", async (
            LoginRequest request, 
            AuthService authService, 
            IValidator<LoginRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var response = await authService.LoginAsync(request);
            return Results.Ok(response);
        })
        .AllowAnonymous()
        .WithSummary("Login user");

        group.MapPost("/logout", async (
            RefreshTokenRequest request,
            AuthService authService
        ) =>
        {
            await authService.LogoutAsync(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithSummary("Logout user");

        group.MapPost("/refresh", async (
            RefreshTokenRequest request,
            AuthService authService
        ) =>
        {
            var response = await authService.RefreshAsync(request);
            return Results.Ok(response);
        })
        .AllowAnonymous()
        .WithSummary("Refresh access token");
    }
}