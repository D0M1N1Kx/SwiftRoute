using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;
using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static void MapWarehouseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/warehouse");

        group.MapPost("/", async (
            CreateWarehouseRequest request,
            WarehouseService service,
            IValidator<CreateWarehouseRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            try
            {
                var response = await service.AddWarehouse(request);
                return Results.Ok(response);
            } catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Add warehouse");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var warehouses = await db.Warehouses
                .Select(w => new WarehouseResponse
                {
                    Id = w.Id,
                    Name = w.Name,
                    Address = w.Address,
                    City = w.City,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt
                }).ToListAsync();

            return Results.Ok(warehouses);
        });
    }
}