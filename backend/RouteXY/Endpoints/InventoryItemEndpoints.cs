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
            InventoryService inventoryService
        ) =>
        {
            try {
                var response = await inventoryService.GetInventoryByIdAsync(warehouse);
                return Results.Ok(response);
            } catch (KeyNotFoundException) {
                return Results.NotFound();
            }
        })
        .WithSummary("Get warehouse's inventory by id");

        group.MapGet("/item/{item:guid}", async (
            Guid item,
            InventoryService inventoryService
        ) =>
        {
            try {
                var i = await inventoryService.GetItemByIdAsync(item);
                return Results.Ok(i);
            } catch (KeyNotFoundException) {
                return Results.NotFound();
            }
        })
        .WithSummary("Get inventory item by id");

        group.MapDelete("/item/{i:guid}", async (
            Guid i,
            InventoryService inventoryService
        ) =>
        {
            try {
                await inventoryService.DeleteItemByIdAsync(i);
                return Results.NoContent();
            } catch (KeyNotFoundException) {
                return Results.NotFound();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Delete item by id");

        group.MapPatch("/item/{id:guid}", async (
            Guid id,
            UpdateInventoryItemRequest request,
            InventoryService inventoryService,
            IValidator<UpdateInventoryItemRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            try {
                var item = await inventoryService.UpdateItemByIdAsync(id, request);
                return Results.Ok(item);
            } catch (KeyNotFoundException) {
                return Results.NotFound();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Update item by id");
    }
}