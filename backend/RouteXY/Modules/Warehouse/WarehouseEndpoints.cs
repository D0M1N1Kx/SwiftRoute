using FluentValidation;
using RouteXY.Api.Modules.Warehouse.Requests;

namespace RouteXY.Api.Modules.Warehouse;

public static class WarehouseEndpoints
{
    public static void MapWarehouseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/warehouse").RequireAuthorization();

        group.MapPost("/", async (
            CreateWarehouseRequest request,
            WarehouseService service,
            IValidator<CreateWarehouseRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var response = await service.AddWarehouseAsync(request);
            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Add warehouse");

        group.MapGet("/", async (WarehouseService warehouseService) =>
        {
            var warehouses = await warehouseService.GetAllAsync();
            return Results.Ok(warehouses);
        })
        .WithSummary("Get all warehouses");

        group.MapGet("/{id:guid}", async (
            Guid id,
            WarehouseService warehouseService
        ) =>
        {
            var warehouse = await warehouseService.GetByIdAsync(id);
            return warehouse == null ? Results.NotFound() : Results.Ok(warehouse);
        })
        .WithSummary("Get warehouse by id");

        group.MapPatch("/{id:guid}", async (
            Guid id,
            WarehouseService warehouseService,
            UpdateWarehouseRequest request,
            IValidator<UpdateWarehouseRequest> validator
        ) =>
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            await warehouseService.UpdateWarehouseAsync(id, request);
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Modify warehouse");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            WarehouseService warehouseService
        ) =>
        {
            await warehouseService.DeleteWarehouseAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Delete warehouse by id");
    }
}