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
        })
        .RequireAuthorization()
        .WithSummary("Get all warehouses");

        group.MapGet("/{id:guid}", async (
            Guid id,
            AppDbContext db
        ) =>
        {
            var warehouse = await db.Warehouses.FindAsync(id);

            if (warehouse == null)
                return Results.NotFound();
            
            return Results.Ok(warehouse);
        })
        .RequireAuthorization()
        .WithSummary("Get warehouse by id");

        group.MapPatch("/{id:guid}", async (
            Guid id,
            AppDbContext db,
            UpdateWarehouseRequest request,
            IValidator<UpdateWarehouseRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var warehouse = await db.Warehouses.FindAsync(id);

            if (warehouse == null)
                return Results.NotFound();

            if (request.Name != null) warehouse.Name = request.Name;
            if (request.Address != null) warehouse.Address = request.Address;
            if (request.City != null) warehouse.City = request.City;
            if (request.Latitude != null) warehouse.Latitude = request.Latitude;
            if (request.Longitude != null) warehouse.Longitude = request.Longitude;
            if (request.IsActive != null) warehouse.IsActive = (bool)request.IsActive;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Modify warehouse");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            AppDbContext db
        ) =>
        {
            var warehouse = await db.Warehouses.FindAsync(id);

            if (warehouse == null)
                return Results.NotFound();
            
            db.Warehouses.Remove(warehouse);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}