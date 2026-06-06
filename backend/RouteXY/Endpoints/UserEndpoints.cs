using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;
using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users").RequireAuthorization();

        group.MapGet("/", async (AppDbContext db) =>
        {
            var users = await db.Users
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }).ToListAsync();

            return Results.Ok(users);
        })
        .WithSummary("Get all users");

        group.MapGet("/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return Results.NotFound();
            
            return Results.Ok(new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        })
        .WithSummary("Get user by ID");

        group.MapPost("/", async (
            CreateUserRequest request,
            AuthService authService,
            IValidator<CreateUserRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            try
            {
                var response = await authService.RegisterAsync(request);
                return Results.Created($"/users/{response.User.Id}", response.User);
            }
            catch (InvalidOperationException ex)
            {
                
                return Results.Conflict(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Create new user");

        group.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return Results.NotFound();
            
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Delete user");
    }
}