using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Hubs;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;

namespace RouteXY.Api.Services;

public class LocationService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<LocationHub> _hubContext;

    public LocationService(AppDbContext db, IHubContext<LocationHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    public async Task UpdateLocationAsync(UpdateCourierLocationRequest request, Guid courierId)
    {
        var location = new CourierLocation
        {
            Id = Guid.NewGuid(),
            CourierId = courierId,
            OrderId = request.OrderId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            SpeedKmh = request.SpeedKmh,
            Heading = request.Heading,
            RecordedAt = DateTime.UtcNow
        };

        _db.CourierLocations.Add(location);
        await _db.SaveChangesAsync();

        await _hubContext.Clients
            .Group("dispatchers")
            .SendAsync("CourierLocationUpdated", new CourierLocationResponse
            {
                CourierId = courierId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                SpeedKmh = request.SpeedKmh,
                Heading = request.Heading,
                RecordedAt = location.RecordedAt
            });
    }

    public async Task<List<CourierLocationResponse>> GetLatestLocationsAsync()
    {
        return await _db.CourierLocations
            .Include(cl => cl.Courier)
            .GroupBy(cl => cl.CourierId)
            .Select(g => g.OrderByDescending(cl => cl.RecordedAt).First())
            .Select(cl => new CourierLocationResponse
            {
                CourierId = cl.CourierId,
                CourierName = cl.Courier.FullName,
                Latitude = cl.Latitude,
                Longitude = cl.Longitude,
                SpeedKmh = cl.SpeedKmh,
                Heading = cl.Heading,
                RecordedAt = cl.RecordedAt
            })
            .ToListAsync();
    }
}