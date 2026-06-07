namespace RouteXY.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static void MapWarehouseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/warehouse");
    }
}