using RouteXY.Api.Data;

namespace RouteXY.Api.Services;

public class WarehouseService
{
    private readonly AppDbContext _db;

    public WarehouseService(AppDbContext db)
    {
        _db = db;
    }
}