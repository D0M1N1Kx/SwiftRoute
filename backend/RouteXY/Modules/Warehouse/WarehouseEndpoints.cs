using FluentValidation;
using RouteXY.Api.Requests;
using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

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
            
            try
            {
                var response = await service.AddWarehouseAsync(request);
                return Results.Ok(response);
            } catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
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

            try
            {
                await warehouseService.UpdateWarehouseAsync(id, request);
                return Results.NoContent();
            } catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Modify warehouse");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            WarehouseService warehouseService
        ) =>
        {
            try
            {
                await warehouseService.DeleteWarehouseAsync(id);
                return Results.NoContent();
            } catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithSummary("Delete warehouse by id");
    }
}