using Microsoft.AspNetCore.SignalR;
using RouteXY.Api.Data;
using RouteXY.Api.Hubs;

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
}