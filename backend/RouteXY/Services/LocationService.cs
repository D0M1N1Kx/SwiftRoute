using Microsoft.AspNetCore.SignalR;
using RouteXY.Api.Data;

namespace RouteXY.Api.Services;

public class LocationService
{
    private readonly AppDbContext _db;

    public LocationService(AppDbContext db)
    {
        _db = db;
    }
}