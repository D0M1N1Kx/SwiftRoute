using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/couriers").RequireAuthorization();

        group.MapGet("/locations", async (LocationService locationService) =>
        {
            var locations = await locationService.GetLatestLocationsAsync();
            return Results.Ok();
        })
        .WithSummary("Get latest location of all couriers");
    }
}