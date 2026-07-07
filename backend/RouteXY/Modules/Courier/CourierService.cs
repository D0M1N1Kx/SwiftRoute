using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Enums;
using RouteXY.Api.Modules.Order.Responses;
using RouteXY.Api.Shared.Services;

namespace RouteXY.Api.Modules.Courier;

public class CourierService(AppDbContext db, TokenService tokenService)
{
    private readonly AppDbContext _db = db;
    private readonly TokenService _tokenService = tokenService;

    public async Task<List<OrderResponse>> GetOwnOrders(Guid userId)
    {
        var orders = await _db.Orders
            .Include(o => o.Dispatcher)
            .Include(o => o.Courier)
            .Where(o => o.CourierId == userId
                && o.Status != OrderStatus.Delivered
                && o.Status != OrderStatus.Cancelled
                && o.Status != OrderStatus.Failed)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => MapToResponse(o))
            .ToListAsync();

        if (orders.Count == 0)
            throw new KeyNotFoundException("Active order not found");
        
        return orders;
    }

    private static OrderResponse MapToResponse(Entities.Order o) => new()
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
}