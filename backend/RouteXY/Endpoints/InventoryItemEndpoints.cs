using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;
using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

public static class InventoryItemEndpoints
{
    public static void MapInventoryItemEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/inventory").RequireAuthorization();

        group.MapPost("/", async (
            CreateInventoryItemRequest request,
            InventoryService service,
            IValidator<CreateInventoryItemRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var response = await service.AddItemAsync(request);
            return Results.Created($"/inventory/{response.Id}", new { response.Id });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher", "WarehouseStaff"))
        .WithSummary("Add inventory item");

        group.MapGet("/{warehouse:guid}", async (
            Guid warehouse,
            AppDbContext db
        ) =>
        {
            var w = await db.Warehouses.FindAsync(warehouse);

            if (w == null)
                return Results.NotFound();
            
            var inventory = await db.InventoryItems.Select(i => new InventoryItemResponse
            {
                Id = i.Id,
                WarehouseId = i.WarehouseId,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                Unit = i.Unit,
                Category = i.Category
            }).ToListAsync();

            return Results.Ok(inventory);
        })
        .WithSummary("Get warehouse's inventory by id");

        group.MapGet("/item/{item:guid}", async (
            Guid item,
            AppDbContext db
        ) =>
        {
            var i = await db.InventoryItems.FindAsync(item);

            if (i == null)
                return Results.NotFound();

            return Results.Ok(i);
        })
        .WithSummary("Get inventory item by id");

        group.MapDelete("/item/{i:guid}", async (
            Guid i,
            AppDbContext db
        ) =>
        {
            var item = await db.InventoryItems.FindAsync(i);

            if (item == null)
                return Results.NotFound();
            
            db.Remove(item);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Delete item by id");

        group.MapPatch("/item/{id:guid}", async (
            Guid id,
            UpdateInventoryItemRequest request,
            AppDbContext db,
            IValidator<UpdateInventoryItemRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var item = await db.InventoryItems.FindAsync(id);

            if (item == null)
                return Results.NotFound();
            
            if (request.Name != null) item.Name = request.Name;
            if (request.Description != null) item.Description = request.Description;
            if (request.Quantity != null) item.Quantity = (int)request.Quantity;
            if (request.Unit != null) item.Unit = request.Unit;
            if (request.Category != null) item.Category = request.Category;

            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.Ok(item);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Update item by id");
    }
}