using System.Security.Claims;
using FluentValidation;
using RouteXY.Api.Requests;
using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/orders").RequireAuthorization();

        group.MapGet("/", async (OrderService orderService) =>
        {
            var orders = await orderService.GetAllAsync();
            return Results.Ok(orders);
        })
        .WithSummary("Get all orders");

        group.MapGet("/{id:guid}", async (Guid id, OrderService orderService) =>
        {
            var order = await orderService.GetByIdAsync(id);
            return order == null ? Results.NotFound() : Results.Ok(order);
        })
        .WithSummary("Get order by ID");

        group.MapPost("/", async (
            CreateOrderRequest request,
            OrderService orderService,
            IValidator<CreateOrderRequest> validator,
            HttpContext context
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var dispatcherId = Guid.Parse(
                context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var order = await orderService.CreateAsync(request, dispatcherId);
            return Results.Created($"/orders/{order.Id}", new { order.Id, order.TrackingNumber });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Create new order");

        group.MapPatch("/{id:guid}", async (
            Guid id,
            AssignCourierRequest request,
            OrderService orderService,
            IValidator<AssignCourierRequest> validator,
            HttpContext context
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var userId = Guid.Parse(
                context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            try
            {
                await orderService.AssignCourierAsync(id, request.CourierId, userId);
                return Results.NoContent();
            } catch (KeyNotFoundException)
            {
                return Results.NotFound();
            } catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Assign courier to order");

        group.MapPatch("/{id:guid}/status", async (
            Guid id,
            UpdateOrderStatusRequest request,
            OrderService orderService,
            IValidator<UpdateOrderStatusRequest> validator,
            HttpContext context
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var userId = Guid.Parse(
                context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            try
            {
                await orderService.UpdateStatusAsync(id, request.Status, request.Note ?? null, userId);
                return Results.NoContent();
            } catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithSummary("Update order status");

        group.MapDelete("/{id:guid}", async (Guid id, OrderService orderService) =>
        {
            try
            {
                await orderService.DeleteAsync(id);
                return Results.NoContent();
            } catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Delete order by id");
    }
}