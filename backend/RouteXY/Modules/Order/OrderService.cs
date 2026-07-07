using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Enums;
using RouteXY.Api.Modules.Order.Requests;
using RouteXY.Api.Modules.Order.Responses;

namespace RouteXY.Api.Modules.Order;

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

    public async Task<Entities.Order> CreateAsync(CreateOrderRequest request, Guid dispatcherId)
    {
        var order = new Entities.Order
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

    public async Task AssignCourierAsync(Guid orderId, Guid courierId, Guid changedBy)
    {
        var order = await _db.Orders.FindAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found");
        
        var courier = await _db.Users.FindAsync(courierId)
            ?? throw new KeyNotFoundException("Courier not found");
        
        if (courier.Role != UserRole.Courier)
            throw new InvalidOperationException("User is not a courier");
        
        var oldStatus = order.Status;
        order.CourierId = courierId;
        order.Status = OrderStatus.Assigned;
        order.UpdatedAt = DateTime.UtcNow;

        _db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ChangedBy = changedBy,
            OldStatus = oldStatus,
            NewStatus = OrderStatus.Assigned,
            ChangedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus, string? note, Guid changedBy)
    {
        var order = await _db.Orders.FindAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found");
        
        var oldStatus = order.Status;
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        if (note != null) order.Notes = note;

        if (newStatus == OrderStatus.Delivered)
            order.DeliveredAt = DateTime.UtcNow;
        
        _db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ChangedBy = changedBy,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Note = note,
            ChangedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid orderId)
    {
        var order = await _db.Orders.FindAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found");
        
        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
    }

    public async Task<List<OrderHistoryResponse>> GetOrderHistory(Guid id)
    {
        var histories = await _db.OrderStatusHistories
            .Include(h => h.ChangedByUser)
            .Where(h => h.OrderId == id)
            .Select(h => MapToResponse(h))
            .ToListAsync();
        
        if (histories.Count == 0)
            throw new KeyNotFoundException("Histories not found");

        return histories;
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

    private static OrderHistoryResponse MapToResponse(Entities.OrderStatusHistory h) => new()
    {
        Id = h.Id,
        OrderId = h.OrderId,
        ChangedById = h.ChangedBy,
        ChangedByName = h.ChangedByUser.FullName,
        OldStatus = h.OldStatus,
        NewStatus = h.NewStatus,
        Note = h.Note,
        ChangedAt = h.ChangedAt
    };

    private static string GenerateTrackingNumber() =>
        $"RXY-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
}