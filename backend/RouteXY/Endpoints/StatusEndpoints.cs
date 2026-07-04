using RouteXY.Api.Services;

namespace RouteXY.Api.Endpoints;

public static class StatusEndpoints
{
    public static void MapStatusEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/status").RequireAuthorization();

        group.MapGet("/workers", async (
            StatusService service) => service.GetWorkerCountsAsRoles());
    }
}