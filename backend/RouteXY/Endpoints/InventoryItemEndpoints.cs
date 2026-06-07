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
        var group = app.MapGroup("/inventory");

        group.MapPost("/", async (
            CreateInventoryItemRequest request,
            WarehouseService service,
            IValidator<CreateInventoryItemRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            try
            {
                var response = await service.AddInventoryItem(request);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole(new string[] { "Admin", "Dispatcher" }))
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
        .RequireAuthorization()
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
        .RequireAuthorization()
        .WithSummary("Get inventory item by id");

        group.MapDelete("/{i:guid}", async (
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
        .RequireAuthorization(policy => policy.RequireRole(new string[] { "Admin", "Dispatcher" }))
        .WithSummary("Delete item by id");
    }
}