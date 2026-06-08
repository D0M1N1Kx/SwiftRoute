using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Responses;

namespace RouteXY.Api.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        return await _db.Orders
            .Include(o => o.Dispatcher)
            .Include(o => o.Courier)
            .Select(o => MapToResponse(o))
            .ToListAsync();
    }

    private static OrderResponse MapToResponse(Order o) => new()
    {
        Id = o.Id,
        TrackingNumber = o.TrackingNumber,
        RecipientName = o.RecipientName,
        RecipientPhone = o.RecipientPhone,
        PickupAddress = o.PickupAddress,
        PickupLat = o.PickupLat,
        PickupLng = o.PickupLng,
        DeliveryAddress = o.DeliveryAddress,
        DeliveryLat = o.DeliveryLat,
        DeliveryLng = o.DeliveryLng,
        Status = o.Status,
        Notes = o.Notes,
        DispatcherName = o.Dispatcher.FullName,
        CourierName = o.Courier?.FullName,
        CreatedAt = o.CreatedAt,
        DeliveredAt = o.DeliveredAt
    };

    private static string GenerateTrackingNumber() =>
        $"RXY-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
}