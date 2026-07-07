using System.Security.Claims;

namespace RouteXY.Api.Modules.Courier;

public static class CourierEndpoints
{
    public static void MapCourierEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/courier")
            .RequireAuthorization(policy => policy.RequireRole("Courier"));
        
        group.MapGet("/me/orders", async (
            HttpContext context,
            CourierService service
        ) =>
        {
            var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Results.Ok(await service.GetOwnOrders(userId));
        });
    }
}