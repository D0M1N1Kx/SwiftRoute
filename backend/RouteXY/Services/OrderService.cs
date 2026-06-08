using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Enums;
using RouteXY.Api.Requests;
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

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await _db.Orders
            .Include(o => o.Dispatcher)
            .Include(o => o.Courier)
            .FirstOrDefaultAsync(o => o.Id == id);
        
        return order == null ? null : MapToResponse(order);
    }

    public async Task<Order> CreateAsync(CreateOrderRequest request, Guid dispatcherId)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            TrackingNumber = GenerateTrackingNumber(),
            DispatcherId = dispatcherId,
            RecipientName = request.RecipientName,
            RecipientPhone = request.RecipientPhone,
            PickupAddress = request.PickupAddress,
            PickupLat = request.PickupLat,
            PickupLng = request.PickupLng,
            DeliveryAddress = request.DeliveryAddress,
            DeliveryLat = request.DeliveryLat,
            DeliveryLng = request.DeliveryLng,
            WarehouseId = request.WarehouseId,
            InventoryItemId = request.InventoryItemId,
            Notes = request.Notes,
            Status = OrderStatus.Pending
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
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