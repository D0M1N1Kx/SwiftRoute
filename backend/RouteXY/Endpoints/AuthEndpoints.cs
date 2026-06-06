using FluentValidation;
using RouteXY.Api.Requests;
using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

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
            
            try
            {
                var response = await authService.LoginAsync(request);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
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
            try
            {
                var response = await authService.RefreshAsync(request);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .AllowAnonymous()
        .WithSummary("Refresh access token");
    }
}