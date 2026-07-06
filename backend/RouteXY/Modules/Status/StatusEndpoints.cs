namespace RouteXY.Api.Modules.Status;

public static class StatusEndpoints
{
    public static void MapStatusEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/status").RequireAuthorization();

        group.MapGet("/workers", async (
            StatusService service) => await service.GetWorkerCountsByRole())
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}