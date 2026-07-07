using System.Security.Claims;
using FluentValidation;
using RouteXY.Api.Modules.Order.Requests;

namespace RouteXY.Api.Modules.Order;

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
            
            await orderService.AssignCourierAsync(id, request.CourierId, userId); 
            return Results.NoContent();
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
            
            await orderService.UpdateStatusAsync(id, request.Status, request.Note ?? null, userId);
            return Results.NoContent();
        })
        .WithSummary("Update order status");

        group.MapDelete("/{id:guid}", async (Guid id, OrderService orderService) =>
        {
            await orderService.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin", "Dispatcher"))
        .WithSummary("Delete order by id");

        group.MapGet("/{id:guid}/history", async (
            Guid id,
            OrderService service
        ) => Results.Ok(await service.GetOrderHistory(id)));
    }
}