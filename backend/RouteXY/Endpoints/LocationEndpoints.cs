using System.Security.Claims;
using FluentValidation;
using RouteXY.Api.Requests;
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
            return Results.Ok(locations);
        })
        .WithSummary("Get latest location of all couriers");

        group.MapPost("/location", async (
            UpdateCourierLocationRequest request,
            LocationService locationService,
            IValidator<UpdateCourierLocationRequest> validator,
            HttpContext context
        ) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());
            
            var courierId = Guid.Parse(
                context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            await locationService.UpdateLocationAsync(request, courierId);
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole("Courier", "Driver", "Admin"))
        .WithSummary("Update courier location");
    }
}